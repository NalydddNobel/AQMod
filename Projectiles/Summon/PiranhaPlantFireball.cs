using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class PiranhaPlantFireball : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void AI()
        {
            projectile.rotation = MathHelper.PiOver2 * (Main.GameUpdateCount % 24 / 6);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }
    }
}