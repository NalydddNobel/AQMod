using AQMod.Common;
using Terraria;
using Terraria.ID;

namespace AQMod.Content.UserInterface.ItemSlots
{
    public static class ItemSlotInteractionHelper
    {
        public static bool CanSwapItem(Item slotItem, Item mouseItem)
        {
            return !slotItem.IsAir || !mouseItem.IsAir;
        }

        public static void SwapItem(ref Item slotItem, ref Item mouseItem)
        {
            Item slotClone = slotItem.Clone();
            slotItem = mouseItem;
            mouseItem = slotClone;
            Main.PlaySound(SoundID.Grab);
        }

        public static void HandleItemSlotMouseInteractions(ref Item slotItem)
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

        public static bool CanSwapItem_SingleStack(Item slotItem, Item mouseItem)
        {
            return slotItem.type > ItemID.None ? mouseItem.type == ItemID.None || mouseItem.stack <= 1 : mouseItem.type > ItemID.None;
        }

        public static void SwapItem_SingleStack(ref Item slotItem, ref Item mouseItem)
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

        public static void HandleItemSlotMouseInteractions_SingleStack(ref Item slotItem)
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
                    if (CanSwapItem_SingleStack(slotItem, Main.mouseItem))
                        SwapItem_SingleStack(ref slotItem, ref Main.mouseItem);
                }
            }
            else if (Main.mouseRight && Main.mouseRightRelease)
            {

            }
        }

        public static bool CanSwapItem_NameTag(Item slotItem, Item mouseItem)
        {
            if (mouseItem != null && !mouseItem.IsAir && mouseItem.maxStack == 1 && !AQItem.Sets.CantBeRenamed[mouseItem.type])
                return true;
            if (slotItem != null && !slotItem.IsAir && (mouseItem == null || mouseItem.IsAir))
                return true;
            return false;
        }

        public static void SwapItem_NameTag(ref Item slotItem, ref Item mouseItem)
        {
            if (mouseItem.stack <= 1)
            {
                Item slotClone = slotItem.Clone();
                slotItem = mouseItem;
                mouseItem = slotClone;
            }
            Main.PlaySound(SoundID.Grab);
        }

        public static void HandleItemSlotMouseInteractions_NameTag(ref Item slotItem)
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
                    if (CanSwapItem_NameTag(slotItem, Main.mouseItem))
                        SwapItem_NameTag(ref slotItem, ref Main.mouseItem);
                }
            }
            else if (Main.mouseRight && Main.mouseRightRelease)
            {

            }
        }
    }
}