using Aequus.Common.Items;
using Aequus.Systems.Shimmer;

namespace Aequus.Content.Items.Accessories.Balloons;

[AutoloadEquip(EquipType.Balloon)]
public class SlimyBlueBalloon : ModItem {
    public static readonly float MaxFallSpeedMultiplier = 0.5f;
    public static readonly float FallGravityMultiplier = 0.5f;

    public override void SetStaticDefaults() {
        Shimmer.SetTransformation(from: ItemID.ShinyRedBalloon, into: Type);
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = ItemDefaults.NPCSkyMerchant;
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        ApplyEffects(player, MaxFallSpeedMultiplier, FallGravityMultiplier);
    }

    public static void ApplyEffects(Player player, float maxFallSpeedMultiplier, float fallGravityMultiplier) {
        if (player.controlDown) {
            return;
        }

        player.maxFallSpeed *= maxFallSpeedMultiplier;
        if (player.velocity.Y > 0f) {
            player.gravity *= fallGravityMultiplier;
            player.fallStart = (int)(player.position.Y / 16f);
        }
    }
}
