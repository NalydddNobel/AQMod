using Aequus.Common.Tiles.Components;

namespace Aequus.Common.Tiles;

public partial class AequusTile {
    private static void WorldGen_UpdateWorld_UndergroundTile(On_WorldGen.orig_UpdateWorld_UndergroundTile orig, int i, int j, bool checkNPCSpawns, int wallDist) {
        Tile tile = Main.tile[i, j];
        ushort tileType = tile.TileType;

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

    public override void RandomUpdate(int i, int j, int type) {
    }
}