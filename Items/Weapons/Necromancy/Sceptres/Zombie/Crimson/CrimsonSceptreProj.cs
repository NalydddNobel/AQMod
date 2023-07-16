using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Zombie.Crimson {
    public class CrimsonSceptreProj : SceptreAttackProjBase {
        protected override int DustType => ModContent.DustType<CrimsonSceptreParticle>();
        protected override int HitSparkleProjectile => ModContent.ProjectileType<CrimsonSceptreProjOnHit>();
    }
}