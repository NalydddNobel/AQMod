using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc
{
    public class ProtectiveProbe : ModProjectile
    {
        public const int DefenseSlices = 40;
        public const int MaxProbes = 5;
        public const int MaxDefenseSlices = DefenseSlices * MaxProbes;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.3f, 0.5f));

            var sources = Projectile.GetGlobalProjectile<AequusProjectile>();
            if (!Main.player[Projectile.owner].active || Main.player[Projectile.owner].dead || Main.player[Projectile.owner].Aequus().accExpertItemBoostBoCProbesDefense == 0 || sources.MissingProjectileOwner)
            {
                if (Projectile.ai[1] > 300f)
                {
                    Projectile.ai[1]++;
                    Projectile.netUpdate = true;
                    Projectile.timeLeft = Math.Min(Projectile.timeLeft, 20);
                }
                else
                {
                    Projectile.ai[1] += Main.rand.NextFloat(1f, 3f);
                    Projectile.timeLeft = Math.Max(Projectile.timeLeft, 30);
                }
            }
            else
            {
                if (Projectile.ai[1] > 0f)
                {
                    Projectile.ai[1]--;
                    if (Projectile.ai[1] < 0f)
                    {
                        Projectile.ai[1] = 0f;
                    }
                }
                Projectile.timeLeft = Math.Max(Projectile.timeLeft, 30);
            }

            Projectile.CollideWithOthers();

            var owner = Projectile.GetHereditaryOwner();
            var gotoPosition = owner.Center;
            float d = Projectile.Distance(gotoPosition);
            int target = Projectile.FindTargetWithLineOfSight(1200f);
            if (d < (target != -1 ? 50f : 250f))
            {
                if (Projectile.velocity.Length() == 0f)
                {
                    Projectile.velocity = Vector2.One;
                }
                if (Projectile.velocity.Length() > 2f)
                    Projectile.velocity *= 0.985f;

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
                    Projectile.localAI[1]--;
                    if (Projectile.localAI[1] <= 0f)
                    {
                        Projectile.netUpdate = true;
                        Projectile.localAI[1] = 6f;
                    }
                }
            }
            else if (d > 2000f)
            {
                Projectile.Center = gotoPosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            else
            {
                float speed = Math.Max(owner.velocity.Length() * 1.1f, 16f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(gotoPosition - Projectile.Center) * speed, 0.035f);
            }

            if (target != -1 && Projectile.Distance(Main.npc[target].Center) < 500f)
            {
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, (Main.npc[target].Center - Projectile.Center).UnNaN().ToRotation() + MathHelper.Pi, 0.08f);
            }
            else if (d < 400f)
            {
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, (gotoPosition - Projectile.Center).UnNaN().ToRotation() + MathHelper.Pi, 0.08f);
            }
            else
            {
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() + MathHelper.Pi, 0.08f);
            }

            var p = AequusHelpers.GetHereditaryOwnerPlayer(owner);
            if (p != null)
            {
                var aequus = p.Aequus();
                aequus.accExpertItemBoostBoCProbesDefenseProjectile -= DefenseSlices;
                int old = (int)Projectile.localAI[0];
                Projectile.localAI[0] = aequus.accExpertItemBoostBoCProbesDefenseProjectile;
                if (old > 0)
                {
                    if ((int)Projectile.localAI[0] <= 0f)
                    {
                        Projectile.velocity += Main.rand.NextVector2Unit() * 8f;
                        CombatText.NewText(Projectile.getRect(), new Color(80, 100, 255, 255), DefenseSlices, dot: true);
                        SoundEngine.PlaySound(SoundID.NPCHit4);
                        SoundEngine.PlaySound(SoundID.Item14);
                        for (int i = 0; i < 7; i++)
                        {
                            var d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                            d2.fadeIn = d2.scale + 0.1f;
                            d2.noGravity = true;
                        }
                        for (int i = 0; i < 2; i++)
                        {
                            var d2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                            d2.fadeIn = d2.scale + 0.1f;
                            d2.noGravity = true;
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }

            if (Projectile.localAI[0] < 0f)
            {
                Projectile.alpha = 150;
            }
            else
            {
                Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, 0, 0.05f);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            var glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Main.spriteBatch.Draw(glow, Projectile.position + off - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            var trailColor = new Color(120, 120, 120, 0);
            for (int i = 0; i < trailLength; i++)
            {
                float p = AequusHelpers.CalcProgress(trailLength, i);
                p *= p;
                Main.spriteBatch.Draw(glow, Projectile.oldPos[i] + off - Main.screenPosition, frame, trailColor * p * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * (0.8f + 0.2f * p), SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}