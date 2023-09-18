using Aequus.Common.Items;
using Aequus.Core.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Restoration.GoldenFeather;

public class GoldenFeather : ModItem {
    public static int RespawnTimeAmount = 300;
    public static int LifeRegenerationAmount = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(RespawnTimeAmount), TextHelper.Decimals(LifeRegenerationAmount / 2f));

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        aequusPlayer.teamRegeneration += LifeRegenerationAmount;
        aequusPlayer.teamRespawnTimeFlat = Math.Max(aequusPlayer.teamRespawnTimeFlat - RespawnTimeAmount, -RespawnTimeAmount);
    }
}