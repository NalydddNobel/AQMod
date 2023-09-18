using Aequus.Core.Utilities;
using System;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Aequus.Common.Items;

namespace Aequus.Items.Equipment.Accessories.Restoration.GoldenFeather;

public class GoldenWind : ModItem {
    public static int RespawnTimeAmount = 300;
    public static int LifeRegenerationAmount = 3;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(RespawnTimeAmount), TextHelper.Decimals(LifeRegenerationAmount / 2f));

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem + 1;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        aequusPlayer.teamRegeneration += LifeRegenerationAmount;
        aequusPlayer.teamRespawnTimeFlat = Math.Max(aequusPlayer.teamRespawnTimeFlat - RespawnTimeAmount, -RespawnTimeAmount);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BandofRegeneration)
            .AddIngredient<GoldenFeather>()
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
    }
}