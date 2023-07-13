using Aequus.Common.ModPlayers;

namespace Aequus.Items.Accessories.Combat.OnHitAbility.BoneRing;

public interface IBoneRing : IAccessoryData {
    int DebuffDuration { get; }
}