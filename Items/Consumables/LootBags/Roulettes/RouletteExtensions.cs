using Terraria.ModLoader;

namespace Aequus.Items.Consumables.LootBags.Roulettes
{
    public static class RouletteExtensions
    {
        public static ItemLootBuilder.Drops AddSpecialRouletteItem(this ItemLootBuilder.Drops drops, int itemID, int needsItemToBeRolled, int chance = 1, int min = 1, int max = 1)
        {
            drops.Add(new RouletteDropRule(drops.Item, itemID, needsItemToBeRolled, chance, min, max));
            return drops;
        }
        public static ItemLootBuilder.Drops AddSpecialRouletteItem<T>(this ItemLootBuilder.Drops drops, int needsItemToBeRolled, int chance = 1, int min = 1, int max = 1) where T : ModItem
        {
            return AddSpecialRouletteItem(drops, ModContent.ItemType<T>(), needsItemToBeRolled, chance, min, max);
        }
        public static ItemLootBuilder.Drops AddRouletteItem(this ItemLootBuilder.Drops drops, int itemID, int chance = 1)
        {
            drops.Add(new RouletteDropRule(drops.Item, itemID, itemID, chance));
            return drops;
        }
        public static ItemLootBuilder.Drops AddRouletteItem<T>(this ItemLootBuilder.Drops drops, int chance = 1) where T : ModItem
        {
            return AddRouletteItem(drops, ModContent.ItemType<T>(), chance);
        }
    }
}