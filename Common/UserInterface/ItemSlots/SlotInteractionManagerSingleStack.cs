using Terraria;
using Terraria.ID;

namespace AQMod.Content.UserInterface.ItemSlots
{
    public class SlotInteractionManagerSingleStack : SlotInteractionManager
    {
        public override bool CanSwapItem(Item slotItem, Item mouseItem)
        {
            return slotItem.type > ItemID.None ? mouseItem.type == ItemID.None || mouseItem.stack <= 1 : mouseItem.type > ItemID.None;
        }

        public override void SwapItem(ref Item slotItem, ref Item mouseItem)
        {
            if (mouseItem.stack <= 1)
            {
                Item slotClone = slotItem.Clone();
                slotItem = mouseItem;
                mouseItem = slotClone;
            }
            else
            {
                mouseItem.stack--;
                slotItem = mouseItem.Clone();
                slotItem.stack = 1;
            }
            Main.PlaySound(SoundID.Grab);
        }
    }
}