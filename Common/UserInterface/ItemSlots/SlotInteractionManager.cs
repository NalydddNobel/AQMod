using Terraria;
using Terraria.ID;

namespace AQMod.Content.UserInterface.ItemSlots
{
    public abstract class SlotInteractionManager
    {
        public virtual bool CanSwapItem(Item slotItem, Item mouseItem)
        {
            return slotItem.type > ItemID.None || mouseItem.type > ItemID.None;
        }

        public virtual void SwapItem(ref Item slotItem, ref Item mouseItem)
        {
            Item slotClone = slotItem.Clone();
            slotItem = mouseItem;
            mouseItem = slotClone;
            Main.PlaySound(SoundID.Grab);
        }

        public void HandleItemSlotMouseInteractions(ref Item slotItem)
        {
            if (slotItem == null)
                slotItem = new Item();
            if (Main.mouseItem == null)
            {
                Main.mouseItem = new Item();
            }
            else
            {
                if (Main.mouseItem.stack <= 0)
                    Main.mouseItem = new Item();
            }
            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (Main.mouseItem.type != ItemID.None || slotItem.type != ItemID.None)
                {
                    if (CanSwapItem(slotItem, Main.mouseItem))
                        SwapItem(ref slotItem, ref Main.mouseItem);
                }
            }
            else if (Main.mouseRight && Main.mouseRightRelease)
            {

            }
        }
    }
}