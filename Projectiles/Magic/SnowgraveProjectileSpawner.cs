using AQMod.Assets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public sealed class SnowgraveProjectileSpawner : ModProjectile
    {
        public override string Texture => TexturePaths.Empty;

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.timeLeft = 120;
            projectile.coldDamage = true;
            projectile.hide = true;
            projectile.tileCollide = false;
        }

        public override bool CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            projectile.ai[0]++;
            if (projectile.ai[0] >= 2f)
            {
                projectile.ai[0] = 0f;
            }
            int p = Projectile.NewProjectile(projectile.Center, new Vector2(0f, -28f), ModContent.ProjectileType<SnowgraveProjectile>(), projectile.damage / 30, projectile.knockBack, projectile.owner);
            Main.projectile[p].localAI[0] = Main.projectile[p].width / 6;
            Main.projectile[p].localAI[0] -= AQUtils.Wave(projectile.timeLeft * 0.2f, 0f, 18f);
        }
    }
}