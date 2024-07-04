using AequusRemake.Core.Entities.Tiles.Components;
using System;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>int1 = X, int2 = Y</summary>
    public static Action<int, int> OnRandomTileUpdate { get; internal set; }

    private static void WorldGen_UpdateWorld_UndergroundTile(On_WorldGen.orig_UpdateWorld_UndergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
        Tile tile = Framing.GetTileSafely(i, j);
        ushort tileType = tile.TileType;

        OnRandomTileUpdate?.Invoke(i, j);

        if (tileType < TileID.Count) {
            orig(i, j, checkNPCSpawns, wallDist);
            return;
        }

        ModTile modTile = TileLoader.GetTile(tileType);
        IRandomUpdateOverride randomUpdateOverride = modTile as IRandomUpdateOverride;
        if (randomUpdateOverride?.PreRandomUpdate(i, j) != true) {
            orig(i, j, checkNPCSpawns, wallDist);
        }

        randomUpdateOverride?.PostRandomUpdate(i, j);
    }

    private static void WorldGen_UpdateWorld_OvergroundTile(On_WorldGen.orig_UpdateWorld_OvergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
        Tile tile = Main.tile[i, j];
        ushort tileType = tile.TileType;

        OnRandomTileUpdate?.Invoke(i, j);

        if (tileType < TileID.Count) {
            orig(i, j, checkNPCSpawns, wallDist);
            return;
        }

        ModTile modTile = TileLoader.GetTile(tileType);
        IRandomUpdateOverride randomUpdateOverride = modTile as IRandomUpdateOverride;
        if (randomUpdateOverride?.PreRandomUpdate(i, j) != true) {
            orig(i, j, checkNPCSpawns, wallDist);
        }

        randomUpdateOverride?.PostRandomUpdate(i, j);
    }
}
