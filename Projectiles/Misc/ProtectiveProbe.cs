﻿using Aequus.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc {
    public class ProtectiveProbe : ModProjectile
    {
        public const int DefenseSlices = 40;
        public const int MaxProbes = 5;
        public const int MaxDefenseSlices = DefenseSlices * MaxProbes;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
            PushableEntities.AddProj(Type);
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
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * Helper.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.3f, 0.5f));

            var sources = Projectile.GetGlobalProjectile<AequusProjectile>();
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

            if (Projectile.localAI[0] < 0f)
            {
                Projectile.alpha = 150;
            }
            else
            {
                Projectile.alpha = (int)MathHelper.Lerp(Projectile.alpha, 0, 0.05f);
            }
        }

        public override void OnKill(int timeLeft)
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
            var glow = ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value;
            Main.spriteBatch.Draw(glow, Projectile.position + off - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            var trailColor = new Color(120, 120, 120, 0);
            for (int i = 0; i < trailLength; i++)
            {
                float p = Helper.CalcProgress(trailLength, i);
                p *= p;
                Main.spriteBatch.Draw(glow, Projectile.oldPos[i] + off - Main.screenPosition, frame, trailColor * p * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * (0.8f + 0.2f * p), SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}