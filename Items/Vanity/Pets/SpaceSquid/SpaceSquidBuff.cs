using Aequus.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Vanity.Pets.SpaceSquid {
    /// <summary>
    /// Applied by <see cref="ToySpaceGun"/>
    /// </summary>
    public class SpaceSquidBuff : BasePetBuff {
        protected override int PetProj => ModContent.ProjectileType<SpaceSquidPet>();
    }
}