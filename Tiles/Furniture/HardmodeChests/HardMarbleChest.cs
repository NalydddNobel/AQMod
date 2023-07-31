using Aequus.Common.Tiles;
using Aequus.Content.World;
using Aequus.Tiles.Furniture;
using Aequus.Tiles.Furniture.HardmodeChests;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.HardmodeChests; 
public class HardMarbleChest : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MarbleChest;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<HardMarbleChestTile>());
        Item.value = Item.sellPrice(silver: 10);
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(ItemID.MarbleChest, 5)
            .AddIngredient(ItemID.SoulofNight, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }
}

public class HardMarbleChestTile : BaseChest<HardMarbleChest> {
    public override Color MapColor => new(200, 185, 100);

    public override void SetStaticDefaults() {
        HardmodeChestBoost.CountsAsChest[Type] = new(TileID.Containers, ChestType.Gold);
        ChestType.IsGenericUndergroundChest.Add(new(Type));
        base.SetStaticDefaults();
        DustType = DustID.Marble;
    }
}

#region Trapped Chest
public class HardMarbleChestTrapped : TrappedChestBaseItem<HardMarbleChestTileTrapped, HardMarbleChestTile, HardMarbleChest> {
}

public class HardMarbleChestTileTrapped : TrappedChestBaseTile<HardMarbleChestTile, HardMarbleChest> {
}
#endregion