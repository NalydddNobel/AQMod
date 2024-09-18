﻿#if !CRAB_CREVICE_DISABLE
using Aequus.Common.Recipes;
using Aequus.Content.Biomes.CrabCrevice;
using System;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Items.Misc.GrabBags.Crates;

public class CrabCreviceCrateHard : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 10;
    }

    public override void ModifyItemLoot(ItemLoot itemLoot) {
        var l = new ItemLoot(ItemID.OceanCrateHard, Main.ItemDropsDB).Get(includeGlobalDrops: false);
        foreach (var loot in l) {
            if (loot is AlwaysAtleastOneSuccessDropRule oneFromOptions) {
                int[] options = Array.ConvertAll(CrabCreviceBiome.ChestPrimaryLoot, (l) => l.item);
                itemLoot.Add(ItemDropRule.OneFromOptions(1, options));
                continue;
            }
            itemLoot.Add(loot);
        }
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.OceanCrateHard);
        Item.createTile = ModContent.TileType<FishingCratesTile>();
        Item.placeStyle = FishingCratesTile.CrabCreviceCrateHard;
    }

    public override bool CanRightClick() {
        return true;
    }

    public override void AddRecipes() {
        AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<CrabCreviceCrate>());
    }
}
#endif