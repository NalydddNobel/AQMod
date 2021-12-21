using AQMod.Assets;
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
    public class HeartMoth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 7;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
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
                if (projectile.rotation.Abs() < 0.1f)
                {
                    projectile.rotation = 0f;
                }
                else
                {
                    projectile.rotation = Utils.AngleLerp(projectile.rotation, 0f, 0.01f);
                }
                projectile.velocity *= 0.99f;
            }
            else if (distance < 2000f)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(gotoPos - center) * 20f, 0.03f);
                projectile.rotation = projectile.velocity.X * 0.1f;
            }
            else
            {
                projectile.Center = player.Center;
                projectile.velocity *= 0.5f;
            }
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
            var texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            var frame = new Rectangle(0, frameHeight * projectile.frame, texture.Width, frameHeight - 2);
            Vector2 center = projectile.Center;
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if (AQConfigClient.c_EffectQuality >= 1f)
            {
                var spotlightTexture = AQTextures.Lights[LightTex.Spotlight20x20];
                var spotlightOrig = spotlightTexture.Size() / 2f;
                float scale = projectile.scale * 2f + (float)Math.Sin(Main.GlobalTime * 4f) * 0.1f;
                var spotlightColor = LightColor() * (0.3f * scale);
                spotlightColor.A = 0;
                Main.spriteBatch.Draw(spotlightTexture, center - Main.screenPosition, null, spotlightColor * 0.1f, 0f, spotlightOrig, scale * 2f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, frame, new Color(255, 255, 255, 255), 0f, frame.Size() / 2f, projectile.scale, effects, 0f);
            if (AQConfigClient.c_EffectQuality >= 1f)
            {
                var spotlightTexture = AQTextures.Lights[LightTex.Spotlight20x20];
                var spotlightOrig = spotlightTexture.Size() / 2f;
                float scale = projectile.scale * 2f + (float)Math.Sin(Main.GlobalTime * 4f) * 0.1f;
                var spotlightColor = LightColor() * (0.3f * scale);
                spotlightColor.A = 0;
                Main.spriteBatch.Draw(spotlightTexture, center - Main.screenPosition, null, spotlightColor, 0f, spotlightOrig, scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}