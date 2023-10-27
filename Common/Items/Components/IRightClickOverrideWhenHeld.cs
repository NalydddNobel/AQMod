using Terraria;

namespace Aequus.Common.Items.Components;

public interface IRightClickOverrideWhenHeld {
    bool RightClickOverrideWhileHeld(ref Item heldItem, Item[] inv, int context, int slot, Player player, AequusPlayer aequus);
}