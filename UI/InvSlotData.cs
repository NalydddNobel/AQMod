using System;
using Terraria;
using Terraria.ID;

namespace AQMod.UI
{
    public class InvSlotData
    {
        public readonly Func<Item, Item, bool> CanSwap;

        public int X;
        public int Y;
        public Item Item;

        public InvSlotData()
        {
            CanSwap = (i, i2) => true;
        }

        public InvSlotData(Func<Item, Item, bool> canSwap)
        {
            CanSwap = canSwap;
        }

        public InvSlotData(Item item)
        {
            CanSwap = (i, i2) => true;
            Item = item;
        }

        public InvSlotData(Item item, Func<Item, Item, bool> canSwap)
        {
            CanSwap = canSwap;
            Item = item;
        }

        public InvSlotData(int x, int y)
        {
            X = x;
            Y = y;
            CanSwap = (i, i2) => true;
        }

        public InvSlotData(int x, int y, Func<Item, Item, bool> canSwap)
        {
            X = x;
            Y = y;
            CanSwap = canSwap;
        }

        public InvSlotData(int x, int y, Item item)
        {
            X = x;
            Y = y;
            CanSwap = (i, i2) => true;
            Item = item;
        }

        public InvSlotData(int x, int y, Item item, Func<Item, Item, bool> canSwap)
        {
            X = x;
            Y = y;
            CanSwap = canSwap;
            Item = item;
        }

        public void Update()
        {
            if (Item == null)
            {
                Item = new Item();
            }
            if (Item.type == ItemID.DD2EnergyCrystal)
            {
                Item.TurnToAir();
            }
        }

        public bool HasItem()
        {
            return Item != null && !Item.IsAir;
        }
    }
}