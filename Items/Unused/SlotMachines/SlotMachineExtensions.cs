using Terraria.ModLoader;

namespace Aequus.Items.Unused.SlotMachines
{
    internal static class SlotMachineExtensions
    {
        public static ItemLootBuilder.Drops AddSpecialRouletteItem(this ItemLootBuilder.Drops drops, int itemID, int needsItemToBeRolled, int chance = 1, int min = 1, int max = 1)
        {
            drops.Add(new SlotMachineDropRule(drops.Item, itemID, needsItemToBeRolled, chance, min, max));
            return drops;
        }
        public static ItemLootBuilder.Drops AddSpecialRouletteItem<T>(this ItemLootBuilder.Drops drops, int needsItemToBeRolled, int chance = 1, int min = 1, int max = 1) where T : ModItem
        {
            return drops.AddSpecialRouletteItem(ModContent.ItemType<T>(), needsItemToBeRolled, chance, min, max);
        }
        public static ItemLootBuilder.Drops AddRouletteItem(this ItemLootBuilder.Drops drops, int itemID, int chance = 1)
        {
            drops.Add(new SlotMachineDropRule(drops.Item, itemID, itemID, chance));
            return drops;
        }
        public static ItemLootBuilder.Drops AddRouletteItem<T>(this ItemLootBuilder.Drops drops, int chance = 1) where T : ModItem
        {
            return drops.AddRouletteItem(ModContent.ItemType<T>(), chance);
        }
    }
}