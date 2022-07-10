using Terraria;

namespace Aequus.Biomes.DemonSiege
{
    public struct SacrificeData
    {
        public readonly int OriginalItem;
        public readonly int NewItem;
        public UpgradeProgressionType Progression;

        public SacrificeData(int oldItem, int newItem, UpgradeProgressionType progression)
        {
            OriginalItem = oldItem;
            NewItem = newItem;
            Progression = progression;
        }

        public Item Convert(Item original)
        {
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