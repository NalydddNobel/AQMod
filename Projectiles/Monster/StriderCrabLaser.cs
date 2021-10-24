using AQMod.Assets;
using AQMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public sealed class StriderCrabLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.hostile = true;
            projectile.aiStyle = -1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(lightColor.R / 2, lightColor.G / 2, lightColor.B / 2, 0);
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            if (projectile.velocity.Y > 0f)
                projectile.velocity.Y += projectile.velocity.Y * 0.00015f;
            projectile.rotation = projectile.velocity.ToRotation();
            int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, Dust.dustWater());
            Main.dust[d].scale = 0.9f * projectile.scale;
            Main.dust[d].velocity = projectile.velocity * 0.25f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var center = projectile.Center;
            var origin = new Vector2(texture.Width / 2f, 6f);
            Main.spriteBatch.Draw(texture, center - Main.screenPosition, null, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}