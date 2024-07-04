namespace AequusRemake.Core.Entities.Items.Components;

public interface IHoverSlot {
    bool HoverSlot(Item[] inventory, int context, int slot);
}