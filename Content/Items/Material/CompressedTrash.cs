﻿using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Material;

public class CompressedTrash : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.PlatinumBar;
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.White;
        Item.value = Item.sellPrice(copper: 50);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddRecipeGroup(AequusRecipes.AnyTrash, 3)
            .AddTile(TileID.Extractinator)
            .Register()
            .DisableDecraft();
    }
}