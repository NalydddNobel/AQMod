using AQMod.Assets;
using AQMod.Content.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class UltimaFlare : ModProjectile
    {
        public override string Texture => AQMod.ModName + "/" + TextureCache.None;

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 20;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.995f;
            projectile.velocity.Y -= 0.2f;
            Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<UltimaDust>());
        }
    }
}