using Aequus.Common.Items;

namespace Aequus.Content.Equipment.Accessories.Balloons;

[AutoloadEquip(EquipType.Balloon)]
public class SlimyBlueBalloon : ModItem {
    public static float MaxFallSpeedMultiplier { get; set; } = 0.5f;
    public static float FallGravityMultiplier { get; set; } = 0.5f;

    public override void SetStaticDefaults() {
        ItemSets.ShimmerTransformToItem[Type] = ItemID.ShinyRedBalloon;
        ItemSets.ShimmerTransformToItem[ItemID.ShinyRedBalloon] = Type;
    }

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