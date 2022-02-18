using System;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace AQMod.UI
{
    public class InvSlot : UIState
    {
        public readonly Func<Item, Item, bool> CanSwap;

        public int X;
        public int Y;
        public Item Slot;

        public InvSlot()
        {
            CanSwap = (i, i2) => true;
        }

        public InvSlot(Func<Item, Item, bool> canSwap)
        {
            CanSwap = canSwap;
        }

        public InvSlot(Item item)
        {
            CanSwap = (i, i2) => true;
            Slot = item;
        }

        public InvSlot(Item item, Func<Item, Item, bool> canSwap)
        {
            CanSwap = canSwap;
            Slot = item;
        }

        public InvSlot(int x, int y)
        {
            X = x;
            Y = y;
            CanSwap = (i, i2) => true;
        }

        public InvSlot(int x, int y, Func<Item, Item, bool> canSwap)
        {
            X = x;
            Y = y;
            CanSwap = canSwap;
        }

        public InvSlot(int x, int y, Item item)
        {
            X = x;
            Y = y;
            CanSwap = (i, i2) => true;
            Slot = item;
        }

        public InvSlot(int x, int y, Item item, Func<Item, Item, bool> canSwap)
        {
            X = x;
            Y = y;
            CanSwap = canSwap;
            Slot = item;
        }

        public virtual void Update()
        {
            if (Slot.type == ItemID.DD2EnergyCrystal)
            {
                Slot.TurnToAir();
            }
        }

        public bool HasItem()
        {
            return Slot != null && !Slot.IsAir;
        }
    }
}