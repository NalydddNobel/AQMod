using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Common.UI;

public record struct ItemSlotContext(int Context, int Slot, Item[] Inventory, Vector2 Position, Color LightColor);