using System.Collections.Generic;

namespace Aequus.Common.Buffs;

public class BuffUI : GlobalBuff {
    /// <summary>
    /// Prevents the player from right clicking a certain buff type. This list is cleared right before the local player is updated.
    /// </summary>
    public static readonly List<System.Int32> DisableRightClick = new();

    public override System.Boolean RightClick(System.Int32 type, System.Int32 buffIndex) {
        return !DisableRightClick.Contains(type);
    }
}
