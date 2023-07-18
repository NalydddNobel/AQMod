using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.SpaceSquid {
    /// <summary>
    /// Applied by <see cref="ToySpaceGun"/>
    /// </summary>
    public class SpaceSquidBuff : BasePetBuff {
        protected override int PetProj => ModContent.ProjectileType<SpaceSquidPet>();
    }
}