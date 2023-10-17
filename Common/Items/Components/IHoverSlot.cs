using Terraria;

namespace Aequus.Common.Items.Components;

public interface IHoverSlot {
    bool HoverSlot(Item[] inventory, int context, int slot);
}