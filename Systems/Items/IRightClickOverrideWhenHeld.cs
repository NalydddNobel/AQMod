namespace AequusRemake.Systems.Items;

internal interface IRightClickOverrideWhenHeld {
    /// <returns>true to prevent vanilla right click actions.</returns>
    bool RightClickOverrideWhileHeld(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer AequusRemake);
}
