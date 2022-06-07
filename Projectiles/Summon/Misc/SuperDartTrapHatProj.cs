using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Misc
{
    public sealed class SuperDartTrapHatProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.PoisonDartTrap;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PoisonDartTrap);
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
            Projectile.trap = false;
            AIType = ProjectileID.PoisonDartTrap;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return NPCID.Sets.CountsAsCritter[target.type] ? false : null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 480);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture);
            }
        }
    }
}