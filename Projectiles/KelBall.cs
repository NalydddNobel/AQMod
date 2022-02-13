using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class KelBall : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = -1;
            projectile.friendly = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.02f;
            projectile.rotation += projectile.velocity.X * 0.0628f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.Draw(Main.projectileTexture[projectile.type], projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, Main.projectileTexture[projectile.type].Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}