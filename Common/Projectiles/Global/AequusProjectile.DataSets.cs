using System.Collections.Generic;
using Terraria.ID;

namespace Aequus.Projectiles {
    public partial class AequusProjectile {
        public static readonly HashSet<int> IsStarProjectile = new();
        public static readonly HashSet<int> BlacklistSpecialEffects = new();

        private void Load_DataSets() {
            IsStarProjectile.Add(ProjectileID.FallingStar);
            IsStarProjectile.Add(ProjectileID.StarCannonStar);
            IsStarProjectile.Add(ProjectileID.SuperStar);
            IsStarProjectile.Add(ProjectileID.StarCloakStar);
            IsStarProjectile.Add(ProjectileID.Starfury);
            IsStarProjectile.Add(ProjectileID.StarVeilStar);
            IsStarProjectile.Add(ProjectileID.StarWrath);
            IsStarProjectile.Add(ProjectileID.BeeCloakStar);
            IsStarProjectile.Add(ProjectileID.HallowStar);
            IsStarProjectile.Add(ProjectileID.ManaCloakStar);
            IsStarProjectile.Add(ProjectileID.SuperStarSlash);

            BlacklistSpecialEffects.Add(ProjectileID.MolotovFire);
            BlacklistSpecialEffects.Add(ProjectileID.MolotovFire2);
            BlacklistSpecialEffects.Add(ProjectileID.MolotovFire3);
            BlacklistSpecialEffects.Add(ProjectileID.VilethornBase);
            BlacklistSpecialEffects.Add(ProjectileID.NettleBurstLeft);
            BlacklistSpecialEffects.Add(ProjectileID.NettleBurstRight);
            BlacklistSpecialEffects.Add(ProjectileID.CrystalVileShardShaft);
            BlacklistSpecialEffects.Add(ProjectileID.FallingStar);
            BlacklistSpecialEffects.Add(ProjectileID.FallingStarSpawner);
            BlacklistSpecialEffects.Add(ProjectileID.StardustDragon2);
            BlacklistSpecialEffects.Add(ProjectileID.StardustDragon3);
        }

        private void Unload_DataSets() {
            IsStarProjectile.Clear();
            BlacklistSpecialEffects.Clear();
        }
    }
}