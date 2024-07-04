using AequusRemake.Core.Entities.Tiles.Components;
using System;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static int On_WorldGen_PlaceChest(On_WorldGen.orig_PlaceChest orig, int x, int y, ushort type, bool notNearOtherChests, int style) {
        if (TileLoader.GetTile(type) is IPlaceChestHook placeChest) {
            int result = placeChest.PlaceChest(x, y, style, notNearOtherChests);
            if (result != -2) {
                return Math.Clamp(result, -1, Main.maxChests);
            }
        }
        return orig(x, y, type, notNearOtherChests, style);
    }

    private static void On_WorldGen_PlaceChestDirect(On_WorldGen.orig_PlaceChestDirect orig, int x, int y, ushort type, int style, int id) {
        if (TileLoader.GetTile(type) is IPlaceChestHook placeChest && !placeChest.PlaceChestDirect(x, y, style, id)) {
            return;
        }

        orig(x, y, type, style, id);
    }
}
