using Terraria.Utilities;

namespace Aequus.Common.Items.DropRules {
    public struct ItemDrop {
        public int item;
        public int minStack;
        public int maxStack;

        public ItemDrop(int item, int stack) {
            this.item = item;
            minStack = stack;
            maxStack = stack;
        }

        public ItemDrop(int item, int minStack, int maxStack) {
            this.item = item;
            this.minStack = minStack;
            this.maxStack = maxStack;
        }

        public int RollStack(UnifiedRandom rng) {
            return rng.Next(minStack, maxStack + 1);
        }

        public static implicit operator ItemDrop(int itemID) {
            return new ItemDrop(itemID, 1);
        }
        public static implicit operator ItemDrop((int itemID, int stack) value) {
            return new ItemDrop(value.itemID, value.stack);
        }
        public static implicit operator ItemDrop((int itemID, int minStack, int maxStack) value) {
            return new ItemDrop(value.itemID, value.minStack, value.maxStack);
        }
    }
}