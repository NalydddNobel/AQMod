using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.GoldenFeather;

public class GoldenWind : GoldenFeather {
    public static new int LifeRegenerationAmount { get; set; } = 3;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(-RespawnTimeAmount), TextHelper.Decimals(LifeRegenerationAmount / 2f));

    public override int BuffType => ModContent.BuffType<GoldenWindBuff>();

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