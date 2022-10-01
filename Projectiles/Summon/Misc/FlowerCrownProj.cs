using Aequus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Misc
{
    public class FlowerCrownProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.FlowerPetal;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 600;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return NPCID.Sets.CountsAsCritter[target.type] ? false : null;
        }

        public override void AI()
        {
            if ((int)Projectile.ai[0] == 1)
            {
                if (Projectile.timeLeft > 240)
                {
                    Projectile.timeLeft = 240;
                }
                if (Projectile.timeLeft < 85)
                {
                    Projectile.scale -= 0.0055555f;
                    Projectile.gfxOffY += 0.01f;
                }
            }
            else
            {
                float decrement = Math.Max(Projectile.ai[1] * 2f, 0f);
                Projectile.ai[1]++;
                if (Projectile.position.Y < Main.worldSurface * 16f && Main.tile[((int)Projectile.position.X + Projectile.width / 2) / 16, ((int)Projectile.position.Y + Projectile.height / 2) / 16].WallType == WallID.None)
                {
                    float gotoSpeed = Main.windSpeedCurrent * 5f + AequusHelpers.Wave(Projectile.timeLeft * 0.33f, -2f, 0.2f) * Math.Sign(Main.windSpeedCurrent);
                    Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, gotoSpeed, 0.03f) * (1f - Math.Min(decrement * 0.001f, 0.95f));
                    if (Main.windSpeedCurrent.Abs() > 0.1f)
                    {
                        Projectile.velocity.Y += decrement * 0.002f + AequusHelpers.Wave(Projectile.timeLeft * 0.02f, -0f, 0.1f);
                        goto SkipVelocityY;
                    }
                }
                Projectile.velocity.Y += 0.02f + decrement * 0.00008f;

            SkipVelocityY:
                Projectile.rotation += Math.Max(Projectile.velocity.X * 0.015f, 0.015f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.ai[0] = 1f;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Projectile.type], frameY: Projectile.frame);
            var origin = frame.Size() / 2f;
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f + Projectile.gfxOffY) - Main.screenPosition;

            int trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + offset, frame, lightColor * (1f - progress) * 0.5f, Projectile.rotation, origin, Projectile.scale * (1f - progress), SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, Projectile.position + offset, frame, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}