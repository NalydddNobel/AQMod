namespace Aequus.Common.Items.Components;

public interface IRightClickOverrideWhenHeld {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="heldItem"></param>
    /// <param name="inv"></param>
    /// <param name="context"></param>
    /// <param name="slot"></param>
    /// <param name="player"></param>
    /// <param name="aequus"></param>
    /// <returns>true to prevent vanilla right click actions.</returns>
    bool RightClickOverrideWhileHeld(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus);
}