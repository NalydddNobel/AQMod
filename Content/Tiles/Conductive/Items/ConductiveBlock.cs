using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Conductive.Items;
public class ConductiveBlock : ModItem {
    public virtual int BarItem => ItemID.CopperBar;
    public virtual int TileId => ModContent.TileType<ConductiveBlockTile>();

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(TileId);
        Item.value = Item.buyPrice(silver: 1);
    }

    public override void AddRecipes() {
        CreateRecipe(5)
            .AddIngredient(BarItem, 1)
            .AddTile(TileID.Furnaces)
            .Register();
    }
}