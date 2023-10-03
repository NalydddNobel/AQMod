﻿using Aequus.Common.Items;
using Aequus.Core.Utilities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Restoration.GoldenFeather;

public class GoldenFeather : ModItem {
    public static int RespawnTimeAmount = -300;
    public static int LifeRegenerationAmount = 1;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TextHelper.Seconds(-RespawnTimeAmount), TextHelper.Decimals(LifeRegenerationAmount / 2f));

    public virtual int BuffType => ModContent.BuffType<GoldenFeatherBuff>();

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().accGoldenFeather = Item;
    }
}