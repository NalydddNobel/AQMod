using System.Collections.Generic;

namespace Aequus.Common.Buffs;

public class BuffUI : GlobalBuff {
    /// <summary>
    /// Prevents the player from right clicking a certain buff type. This list is cleared right before the local player is updated.
    /// </summary>
    public static readonly List<int> DisableRightClick = new();

    public override bool RightClick(int type, int buffIndex) {
        return !DisableRightClick.Contains(type);
    }
}
