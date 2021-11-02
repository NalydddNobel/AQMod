using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class MoonMoonMirror : ModProjectile
    {
        public const float LaserLength = 1000f;

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.magic = true;
        }

        private float getLaserScaleFromRot(float rot)
        {
            if (projectile.spriteDirection == -1)
            {
                if (rot <= MathHelper.PiOver2)
                {
                    return projectile.scale * rot / MathHelper.PiOver2 * projectile.scale;
                }
                return 0f;
            }
            else
            {
                if (rot > -MathHelper.PiOver2 && rot < 0f)
                {
                    return rot / -MathHelper.PiOver2 * projectile.scale;
                }
                return 0f;
            }
        }

        public override void AI()
        {
            var player = Main.player[projectile.owner];
            var speed = player.ItemInHand().shootSpeed;
            Vector2 rotatedRelativePoint = player.RotatedRelativePoint(player.MountedCenter);
            if (Main.myPlayer == projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    AQProjectile.UpdateHeldProjDoVelocity(player, rotatedRelativePoint, projectile);
                }
                else
                {
                    projectile.ai[0] = -1f;
                    projectile.timeLeft = player.itemTime;
                    projectile.netUpdate = true;
                }
            }
            projectile.hide = false;
            if ((int)projectile.ai[0] != -1)
            {
                if (player.itemTime <= 2)
                {
                    player.itemTime = 2;
                }
                if (player.itemAnimation <= 2)
                {
                    player.itemAnimation = 2;
                }
                projectile.timeLeft = 2;
            }
            AQProjectile.UpdateHeldProj(player, rotatedRelativePoint, 2f, projectile);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var origin = texture.Size() / 2f;
            var center = projectile.Center;
            var effects = SpriteEffects.None;
            float laserScale = getLaserScaleFromRot(projectile.rotation);
            if (laserScale > 0f)
            {
                var laserStart = center;
                laserScale *= 0.64f;
                if (projectile.spriteDirection == -1)
                    effects = SpriteEffects.FlipHorizontally;
                var laserTexture = ModContent.GetTexture("AQMod/Projectiles/Magic/MoonlightBeam");
                var laserOrig = laserTexture.Size() / 2f;
                float laserRot = projectile.rotation;
                if (projectile.direction == -1)
                    laserRot += MathHelper.Pi;
                var laserColor = new Color(200, 200, 200, 80);
                int amount = (int)(LaserLength / ((laserTexture.Width - 4) * laserScale));
                float speed = (laserOrig.X - 2) * 2f * laserScale;
                var laserDir = default(Vector2);
                var spotlight = TextureCache.Lights[SpotlightID.Spotlight66x66];
                var spotlightOrig = spotlight.Size() / 2f;
                if (projectile.spriteDirection == 1 || projectile.rotation < MathHelper.PiOver2)
                {
                    laserDir = new Vector2(speed, 0f).RotatedBy(laserRot);
                    for (int i = 0; i < amount; i++)
                    {
                        float progress = 1f / amount * i;
                        Vector2 localScale = new Vector2(laserScale, laserScale);
                        if (progress > 0.8f)
                        {
                            localScale = new Vector2(laserScale, laserScale * (1f - (progress - 0.8f) / 0.8f));
                        }
                        var pos = laserStart + laserDir * i - Main.screenPosition;
                        Main.spriteBatch.Draw(laserTexture, pos, null, laserColor, laserRot, origin, localScale, effects, 0f);
                        if (AQMod.EffectQuality >= 1f)
                        {
                            float spotlightIntensity = (float)Math.Sin(Main.GlobalTime * 10f - progress * 20f);
                            Main.spriteBatch.Draw(spotlight, pos, null, new Color(128, 143, 231, 0) * (0.5f + spotlightIntensity * 0.2f), 0f, spotlightOrig, localScale.Y * 1.25f + spotlightIntensity * 0.1f, effects, 0f);
                        }
                    }
                }
                amount /= 3;
                laserStart.Y -= laserOrig.X;
                var normal = Vector2.Normalize(Main.player[projectile.owner].Center + new Vector2(0f, -500f) - laserStart);
                laserDir = normal * speed;
                laserRot = normal.ToRotation();
                for (int i = 0; i < amount; i++)
                {
                    float progress = 1f / amount * i;
                    Main.spriteBatch.Draw(laserTexture, laserStart + laserDir * i - Main.screenPosition + new Vector2(0f, 10f), null, laserColor * (1f - progress), laserRot, origin, new Vector2(laserScale, laserScale * (1f - progress)), effects, 0f);
                }
            }
            origin.X += projectile.spriteDirection * 4f;
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale * 1.25f, effects, 0f);
            return false;
        }
    }
}