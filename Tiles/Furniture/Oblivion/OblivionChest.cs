using Aequus.Tiles.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Oblivion; 

public class OblivionChest : ModItem {
    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<OblivionChestTile>());
        Item.value = Item.sellPrice(silver: 10);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.AshWoodChest)
            .AddTile<OblivionCraftingStationTile>()
            .Register();
    }
}

public class OblivionChestTile : BaseChest<OblivionChest> {
    public override Color MapColor => Color.Red.SaturationMultiply(0.7f);

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        DustType = DustID.Ash;
    }

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
        DrawBasicGlowmask(i, j, spriteBatch, AequusTextures.OblivionChestTile_Glow, Color.White);
    }
}

#region Trapped Chest
public class OblivionChestTrapped : TrappedChestBaseItem<OblivionChestTileTrapped, OblivionChestTile, OblivionChest> {
}

public class OblivionChestTileTrapped : TrappedChestBaseTile<OblivionChestTile, OblivionChest> {
}
#endregion