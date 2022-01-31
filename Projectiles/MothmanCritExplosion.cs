using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class MothmanCritExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 480;
            projectile.height = 480;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 4;
            projectile.hide = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 16;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.Debuffs.BlueFire>(), 480);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly && target.whoAmI != (int)projectile.ai[1];
        }
    }
}