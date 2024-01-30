﻿using Aequus.Common.Items.Components;

namespace Aequus.Content.Equipment.Accessories.GrandReward;

[LegacyName("BusinessCard", "ForgedCard")]
[LegacyName("FaultyCoin", "FoolsGoldRing")]
public class GrandReward : ModItem, IHaveDownsideTip {
    public static System.Single LuckIncrease { get; set; } = 1f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 15);
    }

    public override void UpdateAccessory(Player player, System.Boolean hideVisual) {
        player.GetModPlayer<AequusPlayer>().dropRolls += LuckIncrease;
        player.GetModPlayer<AequusPlayer>().accGrandRewardDownside = true;
    }
}