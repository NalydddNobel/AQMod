namespace Aequus.Common.UI;

public record struct ItemSlotContext(System.Int32 Context, System.Int32 Slot, Item[] Inventory, Vector2 Position, Color LightColor);