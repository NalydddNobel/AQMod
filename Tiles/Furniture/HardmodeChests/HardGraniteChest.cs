using Aequus.Common.Tiles;

namespace Aequus.Tiles.Furniture.HardmodeChests;
public class HardGraniteChest : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.GraniteChest;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<HardGraniteChestTile>());
        Item.value = Item.sellPrice(silver: 10);
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.GraniteChest, 5)
            .AddIngredient(ItemID.SoulofLight, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class HardGraniteChestTile : BaseChest<HardGraniteChest> {
    public override Color MapColor => new(100, 255, 255);

    public override void SetStaticDefaults() {
        ChestType.IsGenericUndergroundChest.Add(new(Type));
        base.SetStaticDefaults();
        DustType = DustID.Granite;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        DrawBasicGlowmask(i, j, spriteBatch, AequusTextures.HardGraniteChestTile_Glow, Color.White);
    }
}

#region Trapped Chest
public class HardGraniteChestTrapped : TrappedChestBaseItem<HardGraniteChestTileTrapped, HardGraniteChestTile, HardGraniteChest> {
}

public class HardGraniteChestTileTrapped : TrappedChestBaseTile<HardGraniteChestTile, HardGraniteChest> {
}
#endregion