using Aequus.Common.Tiles.Components;
using Aequus.Core.ContentGeneration;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_Player_FigureOutWhatToPlace(On_Player.orig_FigureOutWhatToPlace orig, Player self, Tile targetTile, Item sItem, out int tileToCreate, out int previewPlaceStyle, out bool? overrideCanPlace, out int? forcedRandom) {
        orig(self, targetTile, sItem, out tileToCreate, out previewPlaceStyle, out overrideCanPlace, out forcedRandom);

        int checkTile = tileToCreate;
        if (sItem.ModItem is InstancedTileItem instancedTileItem) {
            checkTile = instancedTileItem._modTile.Type;
        }

        ModTile modTile = TileLoader.GetTile(checkTile);
        if (modTile is IOverridePlacement placeOverride) {
            placeOverride.OverridePlacementCheck(self, targetTile, sItem, ref tileToCreate, ref previewPlaceStyle, ref overrideCanPlace, ref forcedRandom);
        }
    }
}
