using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class UltimaFlare : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 20;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
            projectile.hide = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.995f;
            projectile.velocity.Y -= 0.2f;
            Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<UltimaDust>(), 0f, 0f, 0, default(Color), Main.rand.NextFloat(0.9f, 1.35f));
        }
    }
}