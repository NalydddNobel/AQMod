using Aequus.Items;
using Terraria.ModLoader;

namespace Aequus.Unused.Items.SlotMachines {
    internal static class SlotMachineExtensions {
        public static LegacyItemLootBuilder.LegacyDropBuilder AddSpecialRouletteItem(this LegacyItemLootBuilder.LegacyDropBuilder drops, int itemID, int needsItemToBeRolled, int chance = 1, int min = 1, int max = 1) {
            drops.Add(new SlotMachineDropRule(drops.Item, itemID, needsItemToBeRolled, chance, min, max));
            return drops;
        }
        public static LegacyItemLootBuilder.LegacyDropBuilder AddSpecialRouletteItem<T>(this LegacyItemLootBuilder.LegacyDropBuilder drops, int needsItemToBeRolled, int chance = 1, int min = 1, int max = 1) where T : ModItem {
            return drops.AddSpecialRouletteItem(ModContent.ItemType<T>(), needsItemToBeRolled, chance, min, max);
        }
        public static LegacyItemLootBuilder.LegacyDropBuilder AddRouletteItem(this LegacyItemLootBuilder.LegacyDropBuilder drops, int itemID, int chance = 1) {
            drops.Add(new SlotMachineDropRule(drops.Item, itemID, itemID, chance));
            return drops;
        }
        public static LegacyItemLootBuilder.LegacyDropBuilder AddRouletteItem<T>(this LegacyItemLootBuilder.LegacyDropBuilder drops, int chance = 1) where T : ModItem {
            return drops.AddRouletteItem(ModContent.ItemType<T>(), chance);
        }
    }
}