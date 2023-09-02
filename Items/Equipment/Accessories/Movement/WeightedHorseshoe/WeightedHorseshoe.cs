using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Movement.WeightedHorseshoe;

public class WeightedHorseshoe : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        player.maxFallSpeed *= 2f;
        player.GetModPlayer<AequusPlayer>().accWeightedHorseshoe = Item;
        if (player.velocity.Y > 11f) {
            player.GetModPlayer<AequusPlayer>().visualAfterImages = true;
        }
    }
}