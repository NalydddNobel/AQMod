using System.Collections.Generic;

namespace AequusRemake.Core.Entities.Buffs;

public class BuffUI : GlobalBuff {
    /// <summary>Prevents the player from right clicking a certain buff type. This list is cleared right before the local player is updated.</summary>
    public static readonly List<int> DisableRightClick = [];

    public override bool RightClick(int type, int buffIndex) {
        return !DisableRightClick.Contains(type);
    }
}
