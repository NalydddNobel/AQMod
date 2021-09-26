using Terraria;
using Terraria.ID;

namespace AQMod.Content.UserInterface.ItemSlots
{
    public class DyeSlotInteractionManager : SlotInteractionManagerSingleStack
    {
        public override bool CanSwapItem(Item slotItem, Item mouseItem)
        {
            return (mouseItem.dye > 0 || mouseItem.type <= ItemID.None) && base.CanSwapItem(slotItem, mouseItem);
        }
    }
}