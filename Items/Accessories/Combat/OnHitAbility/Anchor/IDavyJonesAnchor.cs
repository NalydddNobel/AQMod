using Aequus.Common.ModPlayers;

namespace Aequus.Items.Accessories.Combat.OnHitAbility.Anchor;

public interface IDavyJonesAnchor : IAccessoryData {
    int AnchorSpawnChance { get; }
}