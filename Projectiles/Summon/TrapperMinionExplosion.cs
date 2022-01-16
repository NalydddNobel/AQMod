using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon
{
    public class TrapperMinionExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.timeLeft = 2;
            projectile.hostile = true;
            projectile.aiStyle = -1;
            projectile.hide = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override void AI()
        {
            if (projectile.ai[0] > 0)
                projectile.active = false;
            projectile.ai[0]++;
        }
    }
}