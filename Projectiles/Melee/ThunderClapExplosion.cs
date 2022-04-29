using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee
{
    public class ThunderClapExplosion : ModProjectile
    {
        public override string Texture => Aequus.TextureNone;
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.timeLeft = 6;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = Projectile.timeLeft * 2;
            Projectile.penetrate = -1;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.IsTheDestroyer())
            {
                Projectile.damage -= 10;
            }
        }
    }
}