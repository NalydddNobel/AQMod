using Aequus.Common.Tiles;

namespace Aequus.Tiles.Furniture.HardmodeChests;
public class HardSandstoneChest : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.DesertChest;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<HardSandstoneChestTile>());
        Item.value = Item.sellPrice(silver: 10);
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.DesertChest, 5)
            .AddIngredient(ItemID.AncientBattleArmorMaterial)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class HardSandstoneChestTile : BaseChest<HardSandstoneChest> {
    public override Color MapColor => new(180, 130, 20);

    public override void SetStaticDefaults() {
        ChestType.IsGenericUndergroundChest.Add(new(Type));
        base.SetStaticDefaults();
        DustType = DustID.Sand;
    }
}

#region Trapped Chest
public class HardSandstoneChestTrapped : TrappedChestBaseItem<HardSandstoneChestTileTrapped, HardSandstoneChestTile, HardSandstoneChest> {
}

public class HardSandstoneChestTileTrapped : TrappedChestBaseTile<HardSandstoneChestTile, HardSandstoneChest> {
}
#endregion