using Aequus.Common.Buffs;
using Terraria.ModLoader;

namespace Aequus.Content.DedicatedContent.Crabs;

/// <summary>
/// Applied by <see cref="FamiliarPickaxe"/>
/// </summary>
public class FamiliarBuff : BasePetBuff {
    protected override int PetProj => ModContent.ProjectileType<FamiliarPet>();
}