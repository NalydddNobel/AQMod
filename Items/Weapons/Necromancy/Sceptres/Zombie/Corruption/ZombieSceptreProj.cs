using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Corruption {
    public class ZombieSceptreProj : SceptreAttackProjBase {
        protected override int DustType => ModContent.DustType<ZombieSceptreParticle>();
        protected override int HitSparkleProjectile => ModContent.ProjectileType<ZombieSceptreProjOnHit>();
    }
}