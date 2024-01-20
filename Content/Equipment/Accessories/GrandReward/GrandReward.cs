﻿using Aequus.Common.Items.Components;

namespace Aequus.Content.Equipment.Accessories.GrandReward;

public class GrandReward : ModItem, IHaveDownsideTip {
    public static float LuckIncrease { get; set; } = 1f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 15);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<AequusPlayer>().dropRolls += LuckIncrease;
        player.GetModPlayer<AequusPlayer>().accGrandRewardDownside = true;
    }
}