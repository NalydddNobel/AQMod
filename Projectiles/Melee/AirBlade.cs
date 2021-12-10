using AQMod;
using AQMod.Assets;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class AirBlade : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.aiStyle = AQProjectile.AIStyles.SpearAI; // this is counted as a spear cause why not?
            projectile.penetrate = -1;
            projectile.alpha = 0;
            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 6;
            projectile.manualDirectionChange = true;
            projectile.ignoreWater = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 255);
        }

        public override void AI()
        {
            var player = Main.player[projectile.owner];
            float speedMultiplier = 1f + (1f - player.meleeSpeed);
            if (projectile.localAI[0] == 0f)
                projectile.localAI[0] = 100f * speedMultiplier;
            var playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            projectile.direction = player.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = player.itemAnimation;
            projectile.position.X = playerCenter.X - projectile.width / 2;
            projectile.position.Y = playerCenter.Y - projectile.height / 2;
            float lerpAmount = MathHelper.Clamp((1f - speedMultiplier) * 3f, 0.1f, 1f);
            if (!player.frozen && !player.stoned)
            {
                if ((int)projectile.ai[0] == 0)
                {
                    projectile.ai[0] = 10f;
                    projectile.ai[1] = Main.rand.NextFloat(-0.23f, 0.23f);
                    if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                        projectile.velocity = Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(projectile.ai[1]) * projectile.ai[0];
                    projectile.netUpdate = true;
                }
                if (player.itemAnimation < player.itemAnimationMax / 3f)
                {
                    projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], 0f, MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                }
                else
                {
                    projectile.ai[0] = MathHelper.Lerp(projectile.ai[0], projectile.localAI[0], MathHelper.Clamp(lerpAmount + 0.55f, 0.8f, 1f));
                }
            }
            if (player.itemAnimation == 0)
                projectile.Kill();
            if (Main.myPlayer == player.whoAmI && lerpAmount > 0f)
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.MouseWorld - playerCenter).RotatedBy(projectile.ai[1]) * projectile.ai[0], lerpAmount * 0.4f);
            projectile.direction = projectile.velocity.X <= 0f ? -1 : 1;
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.ToRotation() + (MathHelper.PiOver4 * 3f);
            if (projectile.spriteDirection == -1)
                projectile.rotation += -MathHelper.PiOver2;
            player.ChangeDir(projectile.direction);
            int d = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(235, 200, 45, 20) * Main.rand.NextFloat(0.8f, 1.25f), 0.8f);
            Main.dust[d].velocity = -projectile.velocity * 0.1f;
            int ghostBlade = ModContent.ProjectileType<AirBladeGhost>();
            if (player.ownedProjectileCounts[ghostBlade] < 3 && Main.rand.NextBool(5))
            {
                int p = Projectile.NewProjectile(projectile.Center + Vector2.Normalize(
                    -projectile.velocity.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4))) * Main.rand.Next(60, 120), projectile.velocity / 2f * speedMultiplier, ghostBlade, projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            var drawPosition = projectile.Center + Vector2.Normalize(projectile.velocity) * (projectile.width / 2f);
            var effect = SpriteEffects.None;
            var origin = new Vector2(10f, 10f);
            if (projectile.direction == -1)
            {
                origin.X = texture.Width - origin.X;
                effect = SpriteEffects.FlipHorizontally;
            }
            lightColor = projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effect, 0f);
            float progress = 0f;
            if (projectile.localAI[0] != 0f)
                progress = projectile.ai[0] / projectile.localAI[0];
                lightColor *= 0.2f + progress * 0.4f;
            lightColor.A = 0;
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, drawPosition + new Vector2(((float)Math.Sin(Main.GlobalTime * 10f) + 1f) + 2f, 0f).RotatedBy(MathHelper.PiOver2 * i) - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, effect, 0f);
            }
            if (projectile.localAI[0] != 0f)
            {
                texture = OldTextureCache.Lights[Assets.Textures.LightTex.Spotlight30x30];
                frame = texture.Frame();
                lightColor = new Color(130, 100, 12, 1) *  AQConfigClient.c_EffectIntensity;
                origin = texture.Size() / 2f;
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress * 0.3f, 0f, origin, projectile.scale, effect, 0f);
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 2f, projectile.scale * 0.2f * progress), effect, 0f);
                Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 0.2f * progress, projectile.scale * 2f), effect, 0f);
                if (AQConfigClient.c_EffectQuality >= 1f)
                {
                    lightColor *= 0.25f;
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress * 0.5f, 0f, origin, projectile.scale, effect, 0f);
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 2f, projectile.scale * 0.2f * progress), effect, 0f);
                    Main.spriteBatch.Draw(texture, drawPosition - Main.screenPosition, frame, lightColor * progress, 0f, origin, new Vector2(projectile.scale * 0.2f * progress, projectile.scale * 2f), effect, 0f);
                }
            }
            return false;
        }
    }
}