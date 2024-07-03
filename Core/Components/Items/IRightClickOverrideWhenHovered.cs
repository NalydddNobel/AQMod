namespace Aequu2.Core.Entities.Items.Components;

internal interface IRightClickOverrideWhenHovered {
    /// <returns>true to prevent vanilla right click actions.</returns>
    bool RightClickOverrideWhenHovered(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer Aequu2);
}