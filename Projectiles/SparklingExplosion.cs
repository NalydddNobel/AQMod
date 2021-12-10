using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class SparklingExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 300;
            projectile.height = 300;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 4;
            projectile.hide = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 16;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.Sparkling>(), 1200);
        }
    }
}