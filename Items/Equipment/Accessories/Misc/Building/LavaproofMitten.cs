﻿using Aequus.Common.Items.EquipmentBooster;

namespace Aequus.Items.Equipment.Accessories.Misc.Building;

public class LavaproofMitten : ModItem {
    public override void SetStaticDefaults() {
        EquipBoostDatabase.Instance.SetNoEffect(Type);
    }

    public override void SetDefaults() {
        Item.DefaultToAccessory();
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(gold: 2, silver: 50);
        Item.hasVanityEffects = true;
    }

    public override void UpdateEquip(Player player) {
        player.Aequus().accLavaPlace = true;
    }
}