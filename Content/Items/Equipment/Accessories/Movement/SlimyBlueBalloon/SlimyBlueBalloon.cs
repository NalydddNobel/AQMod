using Aequus.Common.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Equipment.Accessories.Movement.SlimyBlueBalloon;

[AutoloadEquip(EquipType.Balloon)]
public class SlimyBlueBalloon : ModItem {
    public static float MaxFallSpeedMultiplier = 0.5f;
    public static float FallGravityMultiplier = 0.5f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemCommons.Rarity.SkyMerchantShopItem;
        Item.value = ItemCommons.Price.SkyMerchantShopItem;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (player.controlDown) {
            return;
        }

        player.maxFallSpeed *= MaxFallSpeedMultiplier;
        if (player.velocity.Y > 0f) {
            player.gravity *= FallGravityMultiplier;
            player.fallStart = (int)(player.position.Y / 16f);
        }
    }
}