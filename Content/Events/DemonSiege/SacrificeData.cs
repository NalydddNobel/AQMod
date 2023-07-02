using Terraria;

namespace Aequus.Content.Events.DemonSiege {
    public struct SacrificeData {
        public readonly int OriginalItem;
        public readonly int NewItem;
        public EventTier Progression;
        public bool Hide;
        public bool DisableDecraft;

        public SacrificeData(int oldItem, int newItem, EventTier progression) {
            OriginalItem = oldItem;
            NewItem = newItem;
            Progression = progression;
            Hide = false;
            DisableDecraft = false;
        }

        public Item Convert(Item original) {
            int stack = original.stack;
            int prefix = original.prefix;
            original = original.Clone();
            original.SetDefaults(NewItem);
            original.stack = stack;
            original.Prefix(prefix);
            // TODO: Find a way to preserve global item content?
            return original;
        }
    }
}