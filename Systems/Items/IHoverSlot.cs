namespace AequusRemake.Systems.Items;

public interface IHoverSlot {
    bool HoverSlot(Item[] inventory, int context, int slot);
}