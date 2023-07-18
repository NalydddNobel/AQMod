using Aequus.Common.ModPlayers;

namespace Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.BoneRing;

public interface IBoneRing : IAccessoryData {
    int DebuffDuration { get; }
}