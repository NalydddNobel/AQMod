using System.Collections.Generic;
using System.Linq;

namespace Aequus.Common.Items;

public class ItemScanner {
    private static readonly Dictionary<int, int> _oreTileToBar = new();

    public static int GetBarFromTileId(int tileId) {
        if (_oreTileToBar.TryGetValue(tileId, out int alreadyFoundResult)) {
            return alreadyFoundResult;
        }

        int oreId = 0;

        // Scan items to find one which places this tile.
        for (int i = 0; i < ItemLoader.ItemCount; i++) {
            Item item = ContentSamples.ItemsByType[i];
            if (item.consumable && item.createTile == tileId) {
                oreId = i;
                break;
            }
        }

        int resultId = oreId;

        // Scan through recipes for one which has a result which ends with "Bar", and satisfies other conditions
        for (int i = 0; i < Recipe.numRecipes; i++) {
            Recipe r = Main.recipe[i];
            if (r != null && r.createItem != null && !r.createItem.IsAir && r.createItem.maxStack >= 999
                && r.requiredItem != null && r.requiredItem.Count != 0 && r.requiredItem.Any(i => i != null && i.type == oreId)
                && ItemID.Search.GetName(r.createItem.type).EndsWith("Bar")) {
                resultId = r.createItem.type;
            }
        }

        _oreTileToBar[tileId] = resultId;
        return resultId;
    }
}
