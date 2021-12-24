using AQMod.Content.UserInterface.ItemSlots;
using Terraria;
using Terraria.ID;

namespace AQMod.Content.NameTags
{
    public class NameTagSlot : SlotInteractionManager
    {
        public override bool CanSwapItem(Item slotItem, Item mouseItem)
        {
            if (!mouseItem.IsAir && mouseItem.maxStack == 1 && AQItem.Sets.CanBeRenamed[mouseItem.type])
                return true;
            if (!slotItem.IsAir && mouseItem.IsAir)
                return true;
            return false;
        }

        public override void SwapItem(ref Item slotItem, ref Item mouseItem)
        {
            if (mouseItem.stack <= 1)
            {
                Item slotClone = slotItem.Clone();
                slotItem = mouseItem;
                mouseItem = slotClone;
            }
            Main.PlaySound(SoundID.Grab);
        }
    }
}