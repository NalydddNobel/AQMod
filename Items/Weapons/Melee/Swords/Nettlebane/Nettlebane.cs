﻿using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Aequus.Items.Weapons.Ranged.Guns.Hitscanner;

namespace Aequus.Items.Weapons.Melee.Swords.Nettlebane;

public class Nettlebane : ModItem {
    public override void SetDefaults() {
        Item.DefaultToAequusSword<NettlebaneProj>(40);
        Item.SetWeaponValues(110, 9.5f, 6);
        Item.width = 30;
        Item.height = 30;
        Item.scale = 1f;
        Item.rare = ItemDefaults.RarityPreMechs;
        Item.value = ItemDefaults.ValueEarlyHardmode;
        Item.autoReuse = true;
    }

    public override void HoldItem(Player player) {
        if (player.HasBuff<NettlebaneBuffTier2>() || player.HasBuff<NettlebaneBuffTier3>()) {
            return;
        }

        player.AddBuff(ModContent.BuffType<NettlebaneBuffTier1>(), 2, quiet: true);
    }

    public override bool? UseItem(Player player) {
        Item.FixSwing(player);
        return null;
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override void AddRecipes() {
        AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<Hitscanner>());
    }
}