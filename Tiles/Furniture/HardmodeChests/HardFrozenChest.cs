﻿using Aequus.Common.Tiles;

namespace Aequus.Tiles.Furniture.HardmodeChests;
public class HardFrozenChest : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.FrozenChest;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<HardFrozenChestTile>());
        Item.value = Item.sellPrice(silver: 10);
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.IceChest, 5)
            .AddIngredient(ItemID.FrostCore)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class HardFrozenChestTile : BaseChest<HardFrozenChest> {
    public override Color MapColor => new(105, 115, 255);

    public override void SetStaticDefaults() {
        ChestType.IsGenericUndergroundChest.Add(new(Type));
        base.SetStaticDefaults();
        DustType = DustID.t_Frozen;
    }
}

#region Trapped Chest
public class HardFrozenChestTrapped : TrappedChestBaseItem<HardFrozenChestTileTrapped, HardFrozenChestTile, HardFrozenChest> {
}

public class HardFrozenChestTileTrapped : TrappedChestBaseTile<HardFrozenChestTile, HardFrozenChest> {
}
#endregion