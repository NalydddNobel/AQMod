using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod
{
    internal static class LazyUtils
    {
        public static void drawProjAtCenter(this Projectile projectile)
        {
            projectile.drawProjAtCenter(Lighting.GetColor((int)(projectile.position.X + projectile.width / 2f), (int)(projectile.position.Y + projectile.height / 2f)));
        }

        public static void drawProjAtCenter(this Projectile projectile, Color lightColor)
        {
            var texture = projectile.GetTexture();
            var frame = texture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, frame, projectile.GetAlpha(lightColor), projectile.rotation, frame.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
        }

        public static Texture2D GetTexture(this Projectile Projectile)
        {
            return OldTextureCache.GetProjectile(Projectile.type);
        }
    }
}