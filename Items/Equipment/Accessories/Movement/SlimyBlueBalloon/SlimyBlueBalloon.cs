using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Movement.SlimyBlueBalloon;

[AutoloadEquip(EquipType.Balloon)]
public class SlimyBlueBalloon : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        if (player.controlDown) {
            return;
        }

        player.maxFallSpeed *= 0.5f;
        if (player.velocity.Y > 0f) {
            player.gravity *= 0.5f;
        }
    }
}