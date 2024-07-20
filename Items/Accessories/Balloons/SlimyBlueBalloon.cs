using Aequus.Common.Items;

namespace Aequus.Items.Accessories.Balloons;

[AutoloadEquip(EquipType.Balloon)]
public class SlimyBlueBalloon : ModItem {
    public static float MaxFallSpeedMultiplier { get; set; } = 0.5f;
    public static float FallGravityMultiplier { get; set; } = 0.5f;

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = ItemDefaults.NPCSkyMerchant;
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