using System.Collections.Generic;

namespace Aequus.Common.Tiles;

public partial class AequusTile {
    public static readonly HashSet<int> NoVanillaRandomTickUpdates = new();

    public delegate void RandomUpdateHook(int i, int j, int type);
    public static RandomUpdateHook OnRandomTileUpdate { get; internal set; }

    private static bool DisableRandomTickUpdate(int i, int j) {
        var tile = Framing.GetTileSafely(i, j);
        if (tile.HasTile && NoVanillaRandomTickUpdates.Contains(tile.TileType)) {
            TileLoader.RandomUpdate(i, j, Main.tile[i, j].TileType);
            WallLoader.RandomUpdate(i, j, Main.tile[i, j].WallType);
            return false;
        }
        return true;
    }

    private static void WorldGen_UpdateWorld_UndergroundTile(On_WorldGen.orig_UpdateWorld_UndergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
        if (!DisableRandomTickUpdate(i, j)) {
            return;
        }
        orig(i, j, checkNPCSpawns, wallDist);
    }

    private static void WorldGen_UpdateWorld_OvergroundTile(On_WorldGen.orig_UpdateWorld_OvergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
        if (!DisableRandomTickUpdate(i, j)) {
            return;
        }
        orig(i, j, checkNPCSpawns, wallDist);
    }

    public override void RandomUpdate(int i, int j, int type) {
        OnRandomTileUpdate.Invoke(i, j, type);
    }
}