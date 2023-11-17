using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace Aequus.Content.Tiles.CraftingStations.TrashCompactor;

public record struct TrashCompactorRecipeResults(Recipe Recipe, int DecraftQuantity) {
    public static readonly TrashCompactorRecipeResults None = new(null, 0);

    public bool Invalid => Recipe == null;

    public bool InvalidStack => DecraftQuantity < 1;

    public static TrashCompactorRecipeResults FromItem(Item item) {
        // Cannot get recipe if the item is air or cannot be shimmered.
        if (item.IsAir || !item.CanShimmer()) {
            return None;
        }

        // Only get recipes for items which can place a "Frame Important" (non-block) tile, and isn't a generic torch.
        if (item.createTile > -1 && Main.tileFrameImportant[item.createTile] && !TileID.Sets.Torch[item.createTile]) {
        //if (true) {

            if (item.createTile > -1) {
                // Prevent decrafting for 1x1 tiles which are not light sources like Candles. (Bars, misc)
                var tileObjectData = TileObjectData.GetTileData(item.createTile, item.placeStyle);
                if (tileObjectData != null && tileObjectData.Width == 1 && tileObjectData.Height == 1 && !TileID.Sets.RoomNeeds.CountsAsTorch.ContainsAny(item.createTile)) {
                    return None;
                }
            }

            var resultList = None;
            for (int i = 0; i < Recipe.numRecipes; i++) {
                if (Main.recipe[i] == null || Main.recipe[i].createItem.type != item.type || !Main.recipe[i].NotDisabledAndConditionsMet()) {
                    continue;
                }

                // Item has multiple recipes, cannot decraft!
                if (!resultList.Invalid) {
                    return None;
                }

                resultList = new(Main.recipe[i], item.stack / Main.recipe[i].createItem.stack);
            }

            return resultList;
        }

        return None;
    }
}