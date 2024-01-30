namespace Aequus.Common.Items.Components;

public interface IRightClickOverrideWhenHeld {
    System.Boolean RightClickOverrideWhileHeld(ref Item heldItem, Item[] inv, System.Int32 context, System.Int32 slot, Player player, AequusPlayer aequus);
}