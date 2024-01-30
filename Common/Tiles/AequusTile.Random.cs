using System.Collections.Generic;

namespace Aequus.Common.Tiles;

public partial class AequusTile {
    public static readonly HashSet<System.Int32> NoVanillaRandomTickUpdates = new();

    private static System.Boolean DisableRandomTickUpdate(System.Int32 i, System.Int32 j) {
        var tile = Framing.GetTileSafely(i, j);
        if (tile.HasTile && NoVanillaRandomTickUpdates.Contains(tile.TileType)) {
            TileLoader.RandomUpdate(i, j, Main.tile[i, j].TileType);
            WallLoader.RandomUpdate(i, j, Main.tile[i, j].WallType);
            return false;
        }
        return true;
    }

    private static void WorldGen_UpdateWorld_UndergroundTile(On_WorldGen.orig_UpdateWorld_UndergroundTile orig, System.Int32 i, System.Int32 j, System.Boolean checkNPCSpawns, System.Int32 wallDist) {
        if (!DisableRandomTickUpdate(i, j)) {
            return;
        }
        orig(i, j, checkNPCSpawns, wallDist);
    }

    private static void WorldGen_UpdateWorld_OvergroundTile(On_WorldGen.orig_UpdateWorld_OvergroundTile orig, System.Int32 i, System.Int32 j, System.Boolean checkNPCSpawns, System.Int32 wallDist) {
        if (!DisableRandomTickUpdate(i, j)) {
            return;
        }
        orig(i, j, checkNPCSpawns, wallDist);
    }

    public override void RandomUpdate(System.Int32 i, System.Int32 j, System.Int32 type) {
        //if (Main.hardMode) {
        //    OmniGemTile.Grow(i, j);
        //}
    }
}