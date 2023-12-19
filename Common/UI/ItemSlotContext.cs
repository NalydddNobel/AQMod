using Microsoft.Xna.Framework;

namespace Aequus.Common.UI;

public record struct ItemSlotContext(int Context, int Slot, Item[] Inventory, Vector2 Position, Color LightColor);