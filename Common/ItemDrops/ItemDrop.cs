using Terraria.Utilities;

namespace Aequus.Common.ItemDrops
{
    public struct ItemDrop
    {
        public int item;
        public int minStack;
        public int maxStack;

        public ItemDrop(int item, int stack)
        {
            this.item = item;
            minStack = stack;
            maxStack = stack;
        }

        public ItemDrop(int item, int minStack, int maxStack)
        {
            this.item = item;
            this.minStack = minStack;
            this.maxStack = maxStack;
        }

        public int RollStack(UnifiedRandom rng)
        {
            return rng.Next(minStack, maxStack) + 1;
        }
    }
}