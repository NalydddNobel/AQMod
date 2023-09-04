using Aequus.Common.Items.Components;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Movement.WeightedHorseshoe;

public class WeightedHorseshoe : ModItem, IUpdateItemDye {
    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.buyPrice(gold: 10);
    }

    public override void UpdateAccessory(Player player, bool hideVisual) {
        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        player.maxFallSpeed *= 2f;
        aequusPlayer.accWeightedHorseshoe = Item;
        if (player.velocity.Y > 11f) {
            aequusPlayer.visualAfterImages = true;
        }
    }

    public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        if (isSetToHidden) {
            return;
        }

        var aequusPlayer = player.GetModPlayer<AequusPlayer>();
        aequusPlayer.showHorseshoeAnvilRope = true;
        aequusPlayer.cHorseshoeAnvil = dyeItem.dye;
    }
}