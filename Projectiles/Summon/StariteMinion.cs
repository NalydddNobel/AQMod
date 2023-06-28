using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Buffs.Minion;
using Aequus.Common.Net.Sounds;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon {
    public class StariteMinion : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projPet[Type] = true;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 18;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults() {
            Projectile.netImportant = true;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.minionSlots = 1f;
            Projectile.extraUpdates = 1;
        }

        public override bool MinionContactDamage() => true;
        public override bool? CanCutTiles() => false;

        public override void AI() {
            Player player = Main.player[Projectile.owner];
            var aequus = player.GetModPlayer<AequusPlayer>();
            var center = Projectile.Center;
            Helper.UpdateProjActive<StariteBuff>(Projectile);

            if (!Projectile.tileCollide) {
                var difference = Main.player[Projectile.owner].Center - Projectile.Center;
                if (difference.Length() < 10f || Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height)) {
                    Projectile.tileCollide = true;
                }
                Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, difference, 0.1f)) * Math.Max(6f, Projectile.velocity.Length());
                Projectile.velocity = lerpedVelocity;
                return;
            }

            int target = -1;
            float dist = 800f;
            if (player.HasMinionAttackTargetNPC) {
                var difference = Projectile.Center - center;
                if ((float)Math.Sqrt(difference.X * difference.X + difference.Y * difference.Y) < dist && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, Projectile.position, Projectile.width, Projectile.height)) {
                    target = player.MinionAttackTargetNPC;
                }
            }
            if (target == -1) {
                target = Projectile.FindTargetWithLineOfSight(650f);
            }

            Vector2 gotoPosition = target == -1 ? player.Center : Main.npc[target].Center;

            if ((int)Projectile.ai[0] == 0f) {
                if (Projectile.ai[1] == 0f) {
                    if (Projectile.velocity == Vector2.Zero) {
                        Projectile.velocity = Main.rand.NextVector2Circular(7f, 7f);
                    }
                    Projectile.ai[1] = Main.rand.Next(30, 70);
                    Projectile.netUpdate = true;
                }
                if (Projectile.ai[1] == 1f) {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                float turnSpeed = MathHelper.Clamp(Projectile.ai[1] / 10000f, 0f, 1f);
                Projectile.ai[1]--;
                if (Projectile.localAI[0] > 0)
                    Projectile.localAI[0]--;
                if (turnSpeed != 0f) {
                    float length = Projectile.velocity.Length();
                    Vector2 difference = gotoPosition - center;
                    Vector2 lerpedVelocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, difference, turnSpeed)) * length;
                    Projectile.velocity = lerpedVelocity;
                }
            }
            else if ((int)Projectile.ai[0] == 1f) {
                if (Projectile.localAI[0] == 0f && Projectile.localAI[1] == 0f) {
                    var gotoVelo = new Vector2(Main.rand.NextFloat(4f, 6.5f) + 2f, 0f).RotatedBy((gotoPosition - center).ToRotation());
                    Projectile.localAI[0] = gotoVelo.X;
                    Projectile.localAI[1] = gotoVelo.Y;
                }
                else {
                    var gotoVelo = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
                    float length = gotoVelo.Length();
                    Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, gotoVelo, 0.08f)) * length;
                    bool xCloseEnough = (Projectile.velocity.X - gotoVelo.X).Abs() < 0.1f;
                    bool yCloseEnough = (Projectile.velocity.Y - gotoVelo.Y).Abs() < 0.1f;
                    if (xCloseEnough && yCloseEnough) {
                        Projectile.velocity.X = gotoVelo.X;
                        Projectile.velocity.Y = gotoVelo.Y;
                        Projectile.ai[0] = 0f;
                        Projectile.localAI[0] = 0f;
                        Projectile.localAI[1] = 0f;
                        Projectile.netUpdate = true;
                    }
                }
            }

            if (target == -1) {
                if (!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height)) {
                    Projectile.tileCollide = false;
                }
            }
        }

        public override void PostAI() {
            float size = new Vector2(Projectile.width, Projectile.height).Length();
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Projectile.whoAmI != i && Main.projectile[i].active && (Main.projectile[i].type == Projectile.type || Main.projectile[i].type == Projectile.type)) {
                    var difference = Projectile.Center - Main.projectile[i].Center;
                    float length = difference.Length();
                    if (length < size) {
                        Projectile.velocity += difference * 0.01f;
                    }
                }
            }
            if ((Projectile.Center - Main.player[Projectile.owner].Center).Length() > 2000f) {
                Projectile.Center = Main.player[Projectile.owner].Center;
            }
            if (Main.rand.NextBool(40)) {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MagicMirror);
                Main.dust[d].velocity = Projectile.velocity * 0.01f;
                Main.dust[d].noGravity = true;
            }
            if (Main.rand.NextBool(80)) {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Pink);
                Main.dust[d].velocity.X = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].velocity.Y = Main.rand.NextFloat(-4f, 4f);
                Main.dust[d].noGravity = true;
            }
            if (Main.rand.NextBool(80)) {
                int g = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position + new Vector2(Main.rand.Next(Projectile.width - 4), Main.rand.Next(Projectile.height - 4)), new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), 16 + Main.rand.Next(2));
                Main.gore[g].scale *= 0.6f;
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.2f, 0.2f, 0.1f));
            Projectile.rotation += Projectile.velocity.Length() * 0.0157f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            int dustAmount = hit.SourceDamage / 10;
            if (dustAmount < 1) {
                dustAmount = 1;
            }
            if (hit.Crit) {
                dustAmount *= 2;
            }

            if (Main.rand.NextBool(9)) {
                AequusBuff.ApplyBuff<BlueFire>(target, 120, out bool canPlaySound);
                if (canPlaySound) {
                    ModContent.GetInstance<BlueFireDebuffSound>().Play(target.Center);
                    dustAmount *= 2;
                }
            }
            for (int i = 0; i < dustAmount; i++) {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<MonoSparkleDust>(),
                    Vector2.UnitX.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * (4f + Main.rand.NextFloat() * 4f), 150, new Color(150, 170, 200, 100)).noGravity = true;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.velocity.X != oldVelocity.X) {
                if (oldVelocity.X.Abs() > 2f)
                    Projectile.velocity.X = -oldVelocity.X * 0.8f;
                Projectile.localAI[0] *= -0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y) {
                if (oldVelocity.Y.Abs() > 2f)
                    Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                Projectile.localAI[1] *= -0.8f;
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 255, 255, 250);
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 offset = new Vector2(Projectile.width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = Main.tenthAnniversaryWorld ? Color.DeepPink with { A = 40 } * 0.4f : new Color(35, 80, 150, 40);
            var origin = frame.Size() / 2f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++) {
                float progress = 1f / ProjectileID.Sets.TrailCacheLength[Projectile.type] * i;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset - Main.screenPosition, frame, color * (1f - progress), Projectile.rotation, origin, Math.Max(Projectile.scale * (0.75f + (1f - progress) * 0.25f), 0.1f), effects, 0);
            }

            float time = Main.GameUpdateCount / 240f + Main.GlobalTimeWrappedHourly * 0.04f;
            float globalTimeWrappedHourly2 = Main.GlobalTimeWrappedHourly;
            globalTimeWrappedHourly2 %= 5f;
            globalTimeWrappedHourly2 /= 2.5f;
            if (globalTimeWrappedHourly2 >= 1f) {
                globalTimeWrappedHourly2 = 2f - globalTimeWrappedHourly2;
            }
            globalTimeWrappedHourly2 = globalTimeWrappedHourly2 * 0.5f + 0.5f;

            for (float f = 0f; f < 1f; f += 0.25f) {
                Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition + new Vector2(0f, 8f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(30, 30, 80, 50), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            for (float f = 0f; f < 1f; f += 0.34f) {
                Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition + new Vector2(0f, 4f).RotatedBy((f + time) * (MathHelper.Pi * 2f)) * globalTimeWrappedHourly2, null, new Color(80, 80, 180, 127), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 0) * Helper.Wave(Main.GlobalTimeWrappedHourly * 6f, 0f, 0.5f), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}