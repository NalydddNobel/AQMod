using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.PetsVanity.Familiar {
    /// <summary>
    /// Applied by <see cref="FamiliarPickaxe"/>
    /// </summary>
    public class FamiliarBuff : BasePetBuff {
        protected override int PetProj => ModContent.ProjectileType<FamiliarPet>();
    }
}