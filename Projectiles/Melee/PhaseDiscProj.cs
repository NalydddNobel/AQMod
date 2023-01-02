using Aequus.Particles.Dusts;
using Aequus.Projectiles.Melee.Swords;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class PhaseDiscProj : ValariProj
    {
        public bool IsIce => (int)Projectile.ai[1] == 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 142;
            Projectile.height = 142;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.manualDirectionChange = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 255;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 12;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            var hitbox = Projectile.getRect();
            hitbox.Inflate(16, 16);
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i] != null && Main.item[i].active && Main.item[i].getRect().Intersects(hitbox))
                {
                    if (Main.item[i].Distance(Projectile.Center) < 10f || Projectile.ai[0] > 100f)
                    {
                        Main.item[i].Center = Projectile.Center;
                        Main.item[i].velocity = Projectile.velocity;
                    }
                    else
                    {
                        Main.item[i].velocity = Vector2.Lerp(Main.item[i].velocity,
                            Main.item[i].DirectionTo(Projectile.Center) * Math.Max(16f, Projectile.velocity.Length()), 0.2f);
                    }
                    Main.timeItemSlotCannotBeReusedFor[i] = 2;
                    if (!Main.item[i].instanced)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, i);
                    }
                }
            }
            if (Projectile.localAI[1] > 0.6f && Main.rand.NextBool(2))
            {
                var c = Projectile.frame == 0 ? Color.LightBlue : new Color(255, 222, 40, 255);
                Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation - MathHelper.PiOver2 + (MathHelper.PiOver4 + 0.5f + Main.rand.NextFloat(-1f, 0f)) * Projectile.direction).ToRotationVector2() *
                    Projectile.Size * 0.5f * Main.rand.NextFloat(0.5f, 1.1f),
                    ModContent.DustType<MonoDust>(), Velocity: (Projectile.rotation).ToRotationVector2() * Main.rand.NextFloat(2f, 6f) * Projectile.direction, newColor: c.UseA(150), Scale: Main.rand.NextFloat(0.5f, 2f) * Projectile.localAI[1]);
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.4f, 0.5f) * Projectile.localAI[1]);
            Projectile.rotation += 0.125f * Projectile.direction;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
                if (Projectile.alpha <= 0)
                    Projectile.alpha = 0;
            }
            if (Projectile.ai[1] > 1f)
            {
                if (Projectile.timeLeft < 120)
                {
                    float speed = Math.Max((Main.player[Projectile.owner].Center - Projectile.Center).Length() / 60f, 24f);
                    var l = (Projectile.Center - Main.player[Projectile.owner].Center).Length();
                    if (l < 500f)
                    {
                        Projectile.localAI[1] -= 0.01f;
                        if (Projectile.localAI[1] < 0f)
                        {
                            Projectile.localAI[1] = 0f;
                        }
                        speed *= 1.5f;
                    }
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * speed, Math.Max(1f - (Main.player[Projectile.owner].Center - Projectile.Center).Length() / 40f, 0.04f));
                    Projectile.tileCollide = false;
                    if (l < 20f)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.tileCollide = true;
                    Projectile.localAI[1] += 0.01f;
                    if (Projectile.localAI[1] > 1f)
                    {
                        Projectile.localAI[1] = 1f;
                    }
                }
                Projectile.rotation += 0.04f * Projectile.direction;
                Projectile.ai[0]--;
                if (Projectile.ai[0] < -40f)
                {
                    int target = Projectile.FindTargetWithLineOfSight(500f);
                    var targetLocation = target == -1 ? Main.MouseWorld : Main.npc[target].Center;
                    if (target == -1)
                    {
                        Projectile.netUpdate = true;
                    }
                    if (Projectile.owner != Main.myPlayer)
                        return;
                    var v = (targetLocation - Projectile.Center) / 32f;
                    if (v.Length() > 20f)
                    {
                        v.Normalize();
                        v *= 20f;
                    }
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, v, 0.03f);
                }
                if (Projectile.ai[1] > 2f)
                {
                    int identity = (int)(Projectile.ai[1] - 3);
                    int proj = AequusHelpers.FindProjectileIdentity(Projectile.owner, identity);
                    if (proj != -1)
                    {
                        var v = Main.projectile[proj].Center - Projectile.Center;
                        Projectile.rotation = Main.projectile[proj].rotation + MathHelper.Pi;
                        if (v.Length() > Main.projectile[proj].velocity.Length().UnNaN() * 2f)
                        {
                            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(v) * 12f, 0.1f);
                        }
                        else
                        {
                            Projectile.Center = Main.projectile[proj].Center;
                        }
                    }

                }
                else if (Projectile.ai[0] > -100f)
                {
                    Projectile.velocity *= 0.95f;
                }
                return;
            }
            Projectile.frame = IsIce ? 0 : 1;
            Projectile.ai[0]++;
            if (Projectile.ai[0] > 180f)
            {
                float speed = Math.Max((Main.player[Projectile.owner].Center - Projectile.Center).Length() / 60f, 10f) + Projectile.ai[0] * 0.0003f;
                var l = (Projectile.Center - Main.player[Projectile.owner].Center).Length();
                if (l < 500f)
                {
                    Projectile.localAI[1] -= 0.01f;
                    if (Projectile.localAI[1] < 0f)
                    {
                        Projectile.localAI[1] = 0f;
                    }
                    speed *= 1.5f;
                }
                Projectile.tileCollide = false;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * speed, Math.Max(1f - (Main.player[Projectile.owner].Center - Projectile.Center).Length() / 40f, 0.04f));
                if (l < 20f)
                {
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[0] > 46f)
            {
                Projectile.velocity *= 0.95f;
            }
            if (Projectile.ai[0] < 120f)
            {
                Projectile.localAI[1] += 0.01f;
                if (Projectile.localAI[1] > 1f)
                {
                    Projectile.localAI[1] = 1f;
                }
            }
            hitbox = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && i != Projectile.whoAmI && Main.projectile[i].type == Type && Projectile.owner == Main.projectile[i].owner
                    && (int)Main.projectile[i].ai[1] < 2 && Projectile.Colliding(hitbox, Main.projectile[i].getRect()))
                {
                    Projectile.velocity += Main.projectile[i].DirectionTo(Projectile.Center).UnNaN() * 0.05f;
                    if ((int)Main.projectile[i].ai[1] != (int)Projectile.ai[1] && Projectile.Distance(Main.player[Projectile.owner].Center) > Main.projectile[i].Distance(Main.player[Projectile.owner].Center))
                    {
                        int amt = 20;
                        Main.projectile[i].ai[0] = amt;
                        Main.projectile[i].ai[1] = 3f + Projectile.identity;
                        Main.projectile[i].netUpdate = true;
                        Main.projectile[i].rotation = Projectile.rotation + MathHelper.Pi;
                        Projectile.timeLeft = 2400;
                        Main.projectile[i].timeLeft = 2400;
                        Main.projectile[i].direction = Projectile.direction;
                        Projectile.netUpdate = true;
                        Projectile.ai[1] = 2f;
                        var velocity = Projectile.velocity;
                        Projectile.velocity -= Main.projectile[i].velocity;
                        Main.projectile[i].velocity += velocity;
                        Projectile.velocity *= 0.5f;
                        Main.projectile[i].velocity *= 0.5f;
                        Projectile.knockBack *= 0.33f;
                        Main.projectile[i].knockBack *= 0.33f;
                        return;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Collision.HitTiles(Projectile.position + new Vector2(Projectile.width / 8f, Projectile.height / 8f), oldVelocity, Projectile.width / 5, Projectile.height / 4);
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.realLife > -1)
            {
                Projectile.damage = (int)(Projectile.damage * 0.95f);
            }
            if (Projectile.ai[1] > 0f)
            {
                target.AddBuff(BuffID.Frostburn2, 120);
            }
            if (!IsIce)
            {
                target.AddBuff(BuffID.OnFire3, 120);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = ModContent.Request<Texture2D>(this.GetPath()).Value;
            var slash = SwordProjectileBase.Swish2Texture.Value;
            Projectile.GetDrawInfo(out var _, out var off, out var _, out var origin, out int trailLength);
            var frame = texture.Frame(verticalFrames: 2, frameY: Projectile.frame);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = new Color(90, 80, 90, 40);
            for (int i = 0; i < trailLength; i++)
            {
                float progress = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] + off - Main.screenPosition,
                    frame, color * progress * Projectile.Opacity, Projectile.rotation, origin, Math.Max(Projectile.scale * progress, 0.1f), effects, 0f);
            }

            Main.spriteBatch.Draw(texture, Projectile.position + off - Main.screenPosition,
                frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            var v = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            var c = Projectile.frame == 0 ? Color.LightBlue : new Color(255, 222, 40, 255);
            float slashScale = 1.1f;
            Main.spriteBatch.Draw(slash, Projectile.position + off - Main.screenPosition + v * 40f * Projectile.localAI[1],
                null, c.UseA(0) * Projectile.Opacity * Projectile.localAI[1], Projectile.rotation, slash.Size() / 2f, slashScale * Projectile.scale * Projectile.localAI[1], SpriteEffects.None, 0f);
            return false;
        }
    }
}