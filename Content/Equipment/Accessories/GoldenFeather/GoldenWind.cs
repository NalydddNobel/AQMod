using Aequus.Common.Items;
using Terraria.Localization;

namespace Aequus.Content.Equipment.Accessories.GoldenFeather;

[LegacyName("AloeVera")]
public class GoldenWind : GoldenFeather {
    public static new System.Int32 LifeRegenerationAmount { get; set; } = 3;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ExtendLanguage.Seconds(-RespawnTimeAmount), ExtendLanguage.Decimals(LifeRegenerationAmount / 2f));

    public override System.Int32 BuffType => ModContent.BuffType<GoldenWindBuff>();

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem + 1;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BandofRegeneration)
            .AddIngredient<GoldenFeather>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}