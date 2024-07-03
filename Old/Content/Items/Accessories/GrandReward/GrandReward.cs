using Aequu2.Core.Entities.Items.Components;

namespace Aequu2.Old.Content.Items.Accessories.GrandReward;

[LegacyName("BusinessCard", "ForgedCard")]
[LegacyName("FaultyCoin", "FoolsGoldRing")]
public class GrandReward : ModItem, IHaveDownsideTip {
    public static float LuckIncrease { get; set; } = 1f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Orange;
        Item.value = Item.buyPrice(gold: 15);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.GetModPlayer<Aequu2Player>().dropRolls += LuckIncrease;
        player.GetModPlayer<Aequu2Player>().accGrandRewardDownside = true;
    }
}