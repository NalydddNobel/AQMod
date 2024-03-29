﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.MossCaves.Radon.Brick;

public class RadonMossBrickWallItem : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 400;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableWall(ModContent.WallType<RadonMossBrickWallPlaced>());
    }

    public override void AddRecipes() {
        CreateRecipe(4)
            .AddIngredient<RadonMossBrickItem>()
            .AddTile(TileID.WorkBenches)
            .TryRegisterAfter(ItemID.RainbowMossBlockWall);
        Recipe.Create(ModContent.ItemType<RadonMossBrickItem>())
            .AddIngredient(Type, 4)
            .AddTile(TileID.WorkBenches)
            .TryRegisterAfter(ItemID.RainbowMossBlockWall);
    }
}