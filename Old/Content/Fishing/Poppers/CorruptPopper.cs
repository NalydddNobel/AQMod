using Aequus.Common.Items.Components;
using Aequus.Content.Fishing;
using Terraria.Localization;

namespace Aequus.Old.Content.Fishing.Poppers;

[LegacyName("CursedPopper")]
public class CorruptPopper : ModBait, IModifyFishingPower {
    public static float IncreasedFishingPowerInCorruption { get; set; } = 0.3f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(IncreasedFishingPowerInCorruption));

    public override void SetDefaults() {
        Item.bait = 30;
        Item.value = Item.sellPrice(silver: 10);
        Item.rare = ItemRarityID.LightRed;
        Item.width = 6;
        Item.height = 6;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.UnholyWater, 5)
            .AddIngredient(ItemID.CursedFlame, 1)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.EnchantedNightcrawler);
    }

    public void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel) {
        if (player.ZoneCorrupt) {
            fishingLevel += IncreasedFishingPowerInCorruption;
        }
    }
}