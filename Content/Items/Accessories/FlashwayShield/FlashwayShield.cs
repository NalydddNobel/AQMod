using AequusRemake.Systems.Synergy;

namespace AequusRemake.Content.Items.Accessories.FlashwayShield;

[AutoloadEquip(EquipType.Shield)]
[Replacement("FlashwayNecklace")]
[Replacement("HeartshatterNecklace")]
public class FlashwayShield : ModItem {
    public static float DashSpeed { get; set; } = 14.5f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = Commons.Rare.NPCSkyMerchant;
        Item.value = Commons.Cost.NPCSkyMerchant;
        Item.defense = 2;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (player.dashType != 0 || player.dashDelay != 0) {
            return;
        }

        player.dashType = -1;
        player.GetModPlayer<AequusPlayer>().SetDashData<FlashwayShieldDashData>();
    }
}