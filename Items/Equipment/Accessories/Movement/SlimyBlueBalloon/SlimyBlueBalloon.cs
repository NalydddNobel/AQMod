using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Aequus.Items.Equipment.Accessories.Movement.SlimyBlueBalloon;

[AutoloadEquip(EquipType.Balloon)]
public class SlimyBlueBalloon : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (player.controlDown) {
            return;
        }

        player.maxFallSpeed *= 0.5f;
        if (player.velocity.Y > 0f) {
            player.gravity *= 0.5f;
            player.fallStart = (int)(player.position.Y / 16f);
        }
    }
}