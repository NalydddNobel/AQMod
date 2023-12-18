using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Equipment.Accessories.FlashwayShield;

[AutoloadEquip(EquipType.Shield)]
public class FlashwayShield : ModItem {
    public static float DashSpeed = 14.5f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
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