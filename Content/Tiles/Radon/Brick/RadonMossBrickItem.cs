using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Tiles.Radon.Brick;

public class RadonMossBrickItem : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 100;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(ModContent.TileType<RadonMossBrickCraftedTile>());
    }

    public override void AddRecipes() {
        CreateRecipe(10)
            .AddIngredient<RadonMoss>()
            .AddIngredient(ItemID.ClayBlock, 10)
            .AddTile(TileID.Furnaces)
            .Register();
    }
}