using Aequus;
using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Armor.SetFlowerCrown {
    public class FlowerCrownProj : ModProjectile {
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override bool? CanCutTiles() {
            return false;
        }

        public override bool? CanHitNPC(NPC target) {
            return NPCID.Sets.CountsAsCritter[target.type] ? false : null;
        }

        public override void AI() {
            if ((int)Projectile.ai[0] == 0) {
                Projectile.ai[0] = 1f;
                Projectile.timeLeft += Main.rand.Next(-60, 60);
                Projectile.scale += Main.rand.NextFloat(-0.1f, 0.1f);
                Projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                Projectile.netUpdate = true;
            }
            if (Projectile.timeLeft < 120) {
                Projectile.alpha += 2;
            }

            if ((int)Projectile.ai[0] == 2) {
                if (Projectile.timeLeft > 240) {
                    Projectile.timeLeft = 240;
                }
                if (DrawOriginOffsetY < 2 && Main.rand.NextBool(60)) {
                    DrawOriginOffsetY++;
                }
            }
            else {
                int target = Projectile.FindTargetWithLineOfSight(300f);
                if (target == -1) {
                    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, Main.windSpeedCurrent * 5f, 0.01f);
                }
                else {
                    if (Main.npc[target].Center.X < Projectile.Center.X) {
                        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -1f, 0.05f);
                    }
                    else {
                        Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 1f, 0.05f);
                    }
                }
                Projectile.velocity.Y = Helper.Wave(Projectile.ai[1], 0f, 2f);
                Projectile.ai[1] += 0.04f;
                Projectile.rotation += Projectile.velocity.Length() * 0.01f;

                if (Projectile.localAI[0] == 0) {
                    Projectile.LoopingFrame(9);
                    if (Projectile.frame == 2) {
                        Projectile.localAI[0] = 1f;
                    }
                }
                else {
                    Projectile.BackwardsLoopingFrame(9);
                    if (Projectile.frame == 0) {
                        Projectile.localAI[0] = 0f;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<FlowerCrownWhipTagDebuff>(), 240);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.ai[0] = 2f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            var origin = frame.Size() / 2f;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f + DrawOriginOffsetY) - Main.screenPosition;
            var drawColor = lightColor * Projectile.Opacity;

            int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            for (int i = 0; i < trailLength; i++) {
                float progress = 1f / trailLength * i;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset, frame, drawColor * (1f - progress) * 0.5f, Projectile.rotation, origin, Projectile.scale * (1f - progress), SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.position + offset, frame, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}