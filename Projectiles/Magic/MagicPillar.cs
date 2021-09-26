using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class MagicPillar : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + AQTextureAssets.None;

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 30;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int type = ModContent.DustType<Dusts.MonoDust>();
            var clr = new Color(255, 150, 150, 0);
            Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr, 2f);
            Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr, 2f);
        }
    }
}