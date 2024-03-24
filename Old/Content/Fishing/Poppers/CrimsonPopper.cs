using Aequus.Common.Items.Components;
using Aequus.Content.Fishing;
using Terraria.Localization;

namespace Aequus.Old.Content.Fishing.Poppers;

public class CrimsonPopper : ModBait, IModifyFishingPower {
    public static float IncreasedFishingPowerInCrimson { get; set; } = 0.2f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(IncreasedFishingPowerInCrimson));

    public override void SetDefaults() {
        Item.bait = 40;
        Item.value = Item.sellPrice(silver: 10);
        Item.rare = ItemRarityID.LightRed;
        Item.width = 6;
        Item.height = 6;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.BloodWater, 5)
            .AddIngredient(ItemID.Ichor, 1)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.EnchantedNightcrawler);
    }

    public void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel) {
        if (player.ZoneCrimson) {
            fishingLevel += IncreasedFishingPowerInCrimson;
        }
    }
}