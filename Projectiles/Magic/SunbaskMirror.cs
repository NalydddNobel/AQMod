using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Common;
using AQMod.Common.Utilities;
using AQMod.Content.Dusts;
using AQMod.Effects;
using AQMod.Effects.ScreenEffects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class SunbaskMirror : ModProjectile
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
            float laserScale = getLaserScaleFromRot(projectile.rotation);
            if ((int)projectile.ai[0] != -1)
            {
                if (player.itemTime <= 2)
                {
                    player.itemTime = 2;
                    projectile.friendly = laserScale > 0.05f;
                    if (projectile.friendly)
                    {
                        projectile.ai[0]++;
                        if ((int)projectile.ai[0] > player.itemAnimationMax)
                        {
                            projectile.ai[0] = 0f;
                            var item = player.ItemInHand();
                            if (player.CheckMana(item, item.mana, pay: true, blockQuickMana: false))
                            {
                                var sound = SoundID.DD2_EtherianPortalSpawnEnemy;
                                var pos = projectile.Center;
                                Main.PlaySound(sound.SoundId, (int)pos.X, (int)pos.Y, sound.Style, 1.2f, 0.6f);
                            }
                            else
                            {
                                projectile.ai[0] = -1f;
                            }
                        }
                    }
                }
                else
                {
                    projectile.friendly = false;
                }
                if (player.itemAnimation <= 2)
                {
                    player.itemAnimation = 2;
                }
                projectile.timeLeft = 2;
            }
            AQProjectile.UpdateHeldProj(player, rotatedRelativePoint, 2f, projectile);
            if (AQMod.EffectQuality >= 1f && laserScale > 0.05f)
            {
                float sunProgress = 1f - (player.itemAnimation - 2) / (float)(player.itemAnimationMax - 2);
                if (sunProgress > 0.5f)
                {
                    int amount = (int)(28f * AQMod.EffectQuality);
                    int end = (int)(amount * ((sunProgress - 0.5f) * 2f));
                    int size = (int)(16 * laserScale);
                    var sizeOff = new Vector2(-size / 2f);
                    float speed = LaserLength / amount;
                    float laserRot = projectile.rotation;
                    if (projectile.spriteDirection == -1)
                        laserRot += MathHelper.Pi;
                    var l = Vector2.UnitX.RotatedBy(laserRot + MathHelper.PiOver2);
                    var laserStart = projectile.Center;
                    var laserDir = new Vector2(speed, 0f).RotatedBy(laserRot);
                    for (int i = 0; i < end; i++)
                    {
                        if (Main.rand.NextBool(30))
                        {
                            float progress = 1f / amount * i;
                            Vector2 localScale = new Vector2(laserScale, laserScale);
                            if (progress > 0.8f)
                            {
                                localScale = new Vector2(laserScale, laserScale * (1f - (progress - 0.8f) / 0.8f));
                            }
                            var pos = laserStart + laserDir * i;
                            float spotlightIntensity = (float)Math.Sin(Main.GlobalTime * 10f - progress * 20f);
                            int d = Dust.NewDust(pos + sizeOff, size, size, ModContent.DustType<MonoDust>(), default, default, default, new Color(255, 206, 115, 0), Main.rand.NextFloat(0.75f, 1.35f));
                            Main.dust[d].velocity = l * Main.rand.NextFloat(-4f * laserScale, 4f * laserScale);
                        }
                    }
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            float size = getLaserScaleFromRot(projectile.rotation) * projectile.width;
            float laserRot = projectile.rotation;
            if (projectile.direction == -1)
                laserRot += MathHelper.Pi;
                var normal = new Vector2(1f, 0f).RotatedBy(laserRot);
            Vector2 end = projectile.Center + normal * LaserLength;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, end, size * projectile.scale, ref _);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float scale = getLaserScaleFromRot(projectile.rotation);
            damage = (int)(damage * scale);
            knockback *= scale;
            if (!crit && AQPlayer.PlayerCrit((int)scale * 100, Main.rand))
            {
                crit = true;
            }
            hitDirection = target.position.X + target.width / 2f < Main.player[projectile.owner].position.X + Main.player[projectile.owner].width / 2f ? -1 : 1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var origin = texture.Size() / 2f;
            var center = projectile.Center;
            var effects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                effects = SpriteEffects.FlipHorizontally;
            float laserScale = getLaserScaleFromRot(projectile.rotation);
            var player = Main.player[projectile.owner];
            if (laserScale > 0f)
            {
                float sunProgress = 1f - (player.itemAnimation - 2) / (float)(player.itemAnimationMax - 2);
                if (AQMod.TonsofScreenShakes && Main.myPlayer == projectile.owner)
                {
                    ScreenShakeManager.ChannelEffect("Mirror", new BasicScreenShake(15, (int)(4f * laserScale * sunProgress)));
                }
                var laserStart = center;
                laserScale *= 0.465f;
                var laserTexture = ModContent.GetTexture("AQMod/Projectiles/Magic/SunlightBeam");
                var laserOrig = laserTexture.Size() / 2f;
                float laserRot = projectile.rotation;
                if (projectile.spriteDirection == -1)
                    laserRot += MathHelper.Pi;
                var laserColor = new Color(200, 200, 200, 80);
                int amount = (int)(LaserLength / ((laserTexture.Width - 4) * laserScale));
                float speed = (laserOrig.X - 2) * 2f * laserScale;
                var laserDir = default(Vector2);
                var spotlight = TextureCache.Lights[SpotlightID.Spotlight66x66];
                var spotlightOrig = spotlight.Size() / 2f;
                if (sunProgress > 0.5f && (projectile.spriteDirection == 1 || projectile.rotation < MathHelper.PiOver2))
                {
                    laserDir = new Vector2(speed, 0f).RotatedBy(laserRot);
                    int end = (int)(amount * ((sunProgress - 0.5f) * 2f));
                    for (int i = 0; i < end; i++)
                    {
                        float progress = 1f / amount * i;
                        Vector2 localScale = new Vector2(laserScale, laserScale);
                        if (progress > 0.8f)
                        {
                            localScale = new Vector2(laserScale, laserScale * (1f - (progress - 0.8f) / 0.8f));
                        }
                        var pos = laserStart + laserDir * i - Main.screenPosition;
                        Main.spriteBatch.Draw(laserTexture, pos, null, laserColor, laserRot, origin, localScale, effects, 0f);
                    }
                    if (AQMod.EffectQuality >= 1f && laserScale > 0.05f) // rerun the laser code but draw some cool vfx instead
                    {
                        for (int i = 0; i < end; i++)
                        {
                            float progress = 1f / amount * i;
                            Vector2 localScale = new Vector2(laserScale, laserScale);
                            if (progress > 0.8f)
                            {
                                localScale = new Vector2(laserScale, laserScale * (1f - (progress - 0.8f) / 0.8f));
                            }
                            var pos = laserStart + laserDir * i - Main.screenPosition;
                            float spotlightIntensity = (float)Math.Sin(Main.GlobalTime * 10f - progress * 20f);
                            Main.spriteBatch.Draw(spotlight, pos, null, new Color(255, 150, 40, 0) * (0.5f + spotlightIntensity * 0.2f * localScale.Y), 0f, spotlightOrig, localScale.Y * 1.25f + spotlightIntensity * 0.1f * localScale.Y, effects, 0f);
                        }
                    }
                }
                amount /= 3;
                laserStart.Y -= laserOrig.X;
                var normal = Vector2.Normalize(Main.player[projectile.owner].Center + new Vector2(0f, -500f) - laserStart);
                laserDir = normal * speed;
                laserRot = normal.ToRotation();
                int start = (int)(amount * (sunProgress * 2));
                if (start > amount)
                    start = 0;
                else
                    start = amount - start;
                for (int i = start; i < amount; i++)
                {
                    float progress = 1f / amount * i;
                    Main.spriteBatch.Draw(laserTexture, laserStart + laserDir * i - Main.screenPosition + new Vector2(0f, 10f), null, laserColor * (1f - progress), laserRot, origin, new Vector2(laserScale, laserScale * (1f - progress)), effects, 0f);
                }
                if (AQMod.EffectQuality >= 1f && laserScale > 0.05f)
                {
                    for (int i = start; i < amount; i++)
                    {
                        float progress = 1f / amount * i;
                        var pos = laserStart + laserDir * i - Main.screenPosition + new Vector2(0f, 10f);
                        float spotlightIntensity = (float)Math.Sin(Main.GlobalTime * 10f + progress * 20f);
                        float s = laserScale * (1f - progress);
                        Main.spriteBatch.Draw(spotlight, pos, null, new Color(255, 150, 40, 0) * (0.5f + spotlightIntensity * 0.2f) * (1f - progress), 0f, spotlightOrig, new Vector2(laserScale + spotlightIntensity * 0.1f, s + spotlightIntensity * 0.1f * s) * 1.25f, effects, 0f);
                    }
                }
            }
            origin.X += projectile.spriteDirection * 4f;
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale * 1.25f, effects, 0f);
            return false;
        }
    }
}