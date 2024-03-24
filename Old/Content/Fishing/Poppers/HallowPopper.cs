using Aequus.Common.Items.Components;
using Aequus.Content.Fishing;
using Terraria.Localization;

namespace Aequus.Old.Content.Fishing.Poppers;

[LegacyName("MysticPopper")]
public class HallowPopper : ModBait, IModifyFishingPower {
    public static float IncreasedFishingPowerInHallow { get; set; } = 0.4f;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Percent(IncreasedFishingPowerInHallow));

    public override void SetDefaults() {
        Item.bait = 20;
        Item.value = Item.sellPrice(silver: 12);
        Item.rare = ItemRarityID.LightRed;
        Item.width = 6;
        Item.height = 6;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.HolyWater, 5)
            .AddIngredient(ItemID.UnicornHorn, 1)
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.EnchantedNightcrawler);
    }

    public void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel) {
        if (player.ZoneHallow) {
            fishingLevel += IncreasedFishingPowerInHallow;
        }
    }
}