using Aequus.Common.ModPlayers;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.Anchor;

public interface IDavyJonesAnchor : IAccessoryData {
    int AnchorSpawnChance { get; }
}