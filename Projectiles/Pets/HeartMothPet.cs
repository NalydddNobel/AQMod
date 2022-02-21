using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Pets
{
    public class HeartMothPet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 7;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.aiStyle = -1;
        }

        public Color LightColor()
        {
            return new Color(255, 140, 160, 255);
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            AQPlayer aQPlayer = player.GetModPlayer<AQPlayer>();
            if (player.dead)
                aQPlayer.heartMoth = false;
            if (aQPlayer.heartMoth)
                projectile.timeLeft = 2;
            player.AddBuff(BuffID.HeartLamp, 2);
            Vector2 gotoPos = player.Center + new Vector2(-player.width * 3f * player.direction, -player.height * 1.5f);
            var center = projectile.Center;
            float distance = (center - gotoPos).Length();
            if (distance < projectile.width * 2)
            {
                projectile.velocity *= 0.99f;
            }
            else if (distance < 2000f)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(gotoPos - center) * Math.Max(Main.player[projectile.owner].velocity.Length() + 4f, 10f), 0.015f);
            }
            else
            {
                projectile.Center = player.Center;
                projectile.velocity *= 0.1f;
            }
            projectile.rotation = projectile.velocity.X * 0.075f;
            projectile.frameCounter++;
            if (projectile.frame > 1 && projectile.frame < 4)
            {
                projectile.frameCounter++;
            }
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame >= Main.projFrames[projectile.type])
                    projectile.frame = 0;
            }
            Vector3 lightColor;
            if (Main.player[projectile.owner].cLight > 0)
            {
                lightColor = DyeHelper.ModifyLight(GameShaders.Armor.GetSecondaryShader(Main.player[projectile.owner].cLight, Main.player[projectile.owner]), LightColor().ToVector3());
            }
            else
            {
                lightColor = LightColor().ToVector3();
            }
            Lighting.AddLight(projectile.Center, lightColor);
            projectile.spriteDirection = player.direction;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = Color.White;
            var texture = Main.projectileTexture[projectile.type];
            var frame = texture.Frame(verticalFrames: Main.projFrames[projectile.type], frameY: projectile.frame);
            var origin = frame.Size() / 2f;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f + projectile.gfxOffY) - Main.screenPosition;
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            for (int i = 0; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, new Color(255, 255, 255, 0) * (1f - progress), projectile.rotation, origin, projectile.scale * (1f - progress), effects, 0f);
            }
            if (!AQMod.LowQ)
            {
                var effectTexture = AQTextures.Lights[LightTex.Spotlight66x66];
                var effectFrame = effectTexture.Frame();
                var effectOrigin = effectFrame.Size() / 2f;
                var spotlightColor = LightColor();
                spriteBatch.Draw(effectTexture, projectile.position + offset, effectFrame, spotlightColor * AQUtils.Wave(Main.GlobalTime * 10f, 0.4f, 0.6f), 0f, effectOrigin, projectile.scale * 0.75f, SpriteEffects.None, 0f);
                spriteBatch.Draw(effectTexture, projectile.position + offset, effectFrame, spotlightColor * AQUtils.Wave(Main.GlobalTime * 10f, 0.02f, 0.06f), 0f, effectOrigin, projectile.scale * AQUtils.Wave(Main.GlobalTime, 1f, 1.1f), SpriteEffects.None, 0f);

                Main.spriteBatch.Draw(texture, projectile.position + offset + new Vector2(2f, 0f), frame, new Color(100, 100, 100, 0), projectile.rotation, origin, projectile.scale, effects, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset + new Vector2(-2f, 0f), frame, new Color(100, 100, 100, 0), projectile.rotation, origin, projectile.scale, effects, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset + new Vector2(0, 2f), frame, new Color(100, 100, 100, 0), projectile.rotation, origin, projectile.scale, effects, 0f);
                Main.spriteBatch.Draw(texture, projectile.position + offset + new Vector2(0f, -2f), frame, new Color(100, 100, 100, 0), projectile.rotation, origin, projectile.scale, effects, 0f);
            }
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, lightColor, projectile.rotation, origin, projectile.scale, effects, 0f);
            return false;
        }
    }
}