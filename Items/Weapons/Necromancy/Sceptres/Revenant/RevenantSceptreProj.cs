using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Necromancy.Sceptres.Revenant {
    public class RevenantSceptreProj : SceptreAttackProjBase {
        protected override int DustType => ModContent.DustType<RevenantParticle>();
        protected override int HitSparkleProjectile => ModContent.ProjectileType<RevenantSceptreProjOnHit>();
    }
}