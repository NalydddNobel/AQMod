﻿using Aequus.Common.Items;
using Aequus.Common.Tiles;

namespace Aequus.Tiles.Furniture.HardmodeChests;
public class AdamantiteChest : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.GoldChest;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<AdamantiteChestTile>());
        Item.value = Item.sellPrice(silver: 10);
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.GoldChest, 5)
            .AddIngredient(ItemID.AdamantiteBar, 2)
            .AddTile(TileID.MythrilAnvil)
            .Register()
            .DisableDecraft()
            .Clone()
            .ReplaceItem(ItemID.AdamantiteBar, ItemID.TitaniumBar)
            .Register();
    }
}

public class AdamantiteChestTile : BaseChest<AdamantiteChest> {
    public override Color MapColor => new(160, 25, 50);

    public override void SetStaticDefaults() {
        ChestType.IsGenericUndergroundChest.Add(new(Type));
        base.SetStaticDefaults();
        DustType = DustID.Adamantite;
    }
}

#region Trapped Chest
public class AdamantiteChestTrapped : TrappedChestBaseItem<AdamantiteChestTileTrapped, AdamantiteChestTile, AdamantiteChest> {
}

public class AdamantiteChestTileTrapped : TrappedChestBaseTile<AdamantiteChestTile, AdamantiteChest> {
}
#endregion