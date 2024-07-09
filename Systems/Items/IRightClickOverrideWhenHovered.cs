namespace AequusRemake.Systems.Items;

internal interface IRightClickOverrideWhenHovered {
    /// <returns>true to prevent vanilla right click actions.</returns>
    bool RightClickOverrideWhenHovered(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer AequusRemake);
}