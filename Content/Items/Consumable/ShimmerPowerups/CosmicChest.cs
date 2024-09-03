﻿using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Aequus.Items.Equipment.Accessories.Misc.Luck;

namespace Aequus.Content.Items.Consumable.ShimmerPowerups;

public class CosmicChest : ModItem {
    public static readonly float DropRerolls = 0.05f;

    public override void SetDefaults() {
        Item.DefaultToHoldUpItem();
        Item.width = 24;
        Item.height = 24;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightPurple;
        Item.UseSound = SoundID.Item92;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(gold: 2);
    }

    public override bool? UseItem(Player player) {
        if (!player.Aequus().usedPermaLootLuck) {
            player.Aequus().usedPermaLootLuck = true;
            return true;
        }
        return false;
    }

    public override void AddRecipes() {
        AequusRecipes.AddShimmerCraft(ModContent.ItemType<GrandReward>(), ModContent.ItemType<CosmicChest>(), condition: AequusConditions.DownedOmegaStarite);
    }
}