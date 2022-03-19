using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class FloatstickProj : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1;
                projectile.rotation = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            }
            projectile.velocity *= 0.98f;
            projectile.rotation += projectile.velocity.Length() * 0.0157f;

            if (Main.myPlayer != projectile.owner)
            {
                return;
            }

            Lighting.AddLight(projectile.Center, projectile.GetAlpha(Color.White).ToVector3() * 2f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(190, 40, 220 + (int)(Math.Sin(Main.GlobalTime * 5f) * 40f));
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = projectile.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);

            var drawPosition = projectile.Center - Main.screenPosition;
            for (float f = 0f; f < 1f; f += 0.125f)
            {
                Main.spriteBatch.Draw(texture, drawPosition + new Vector2(2f * projectile.scale, 0f).RotatedBy(Main.GlobalTime + f * MathHelper.TwoPi), frame, projectile.GetAlpha(lightColor) * 0.1f, projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/Lights/Spotlight"), drawPosition, null, projectile.GetAlpha(lightColor) * 0.4f, projectile.rotation, new Vector2(33f, 33f), projectile.scale * AQUtils.Wave(Main.GlobalTime * 2f, 0.4f, 0.5f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(ModContent.GetTexture("AQMod/Assets/Lights/Spotlight"), drawPosition, null, projectile.GetAlpha(lightColor) * 0.08f, projectile.rotation, new Vector2(33f, 33f), projectile.scale * AQUtils.Wave(Main.GlobalTime * 2f, 0.4f, 0.5f) * 2f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(texture, drawPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}