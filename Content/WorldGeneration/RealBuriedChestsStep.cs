using Aequus.Common.WorldGeneration;
using Aequus.Content.Chests.BuriedChests;
using System;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;

public class RealBuriedChestsStep : AequusGenStep {
    public override string InsertAfter => "Buried Chests";

    protected override double GenWeight => 72f;

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        SetMessage(progress);

        int wantedChests = Math.Max(Main.maxTilesX * Main.maxTilesY / 20000, 5);
        for (int i = 0; i < wantedChests; i++) {
            for (int k = 0; k < 10000; k++) {
                int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 50);
                if (TryGenerateChest(x, y)) {
                    break;
                }
            }
        }
    }

    private static bool TryGenerateChest(int x, int y) {
        if (!CheckSolidSpotForChest(x, y)) {
            return false;
        }

        if (CheckStoneSpotForChest(x, y)) {
            return TryGenerateChestInner<CopperChest>(x, y);
        }

        return false;
    }

    private static bool TryGenerateChestInner<T>(int x, int y) where T : UnifiedBuriedChest {
        PunchHoleForChest(x, y);
        int hiddenChestTileId = ModContent.GetInstance<T>().Locked.Hidden.Type;
        WorldGen.PlaceTile(x, y, hiddenChestTileId);
        return Main.tile[x, y].TileType == hiddenChestTileId;
    }

    private static void PunchHoleForChest(int x, int y) {
        for (int i = x; i < x + 2; i++) {
            for (int j = y - 1; j < y + 1; j++) {
                WorldGen.KillTile(i, j);
            }
        }
    }

    private static bool CheckSolidSpotForChest(int x, int y) {
        for (int i = x - 1; i < x + 3; i++) {
            for (int j = y - 2; j < y + 2; j++) {
                if (!Main.tile[i, j].IsFullySolid()) {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool CheckStoneSpotForChest(int x, int y) {
        for (int i = x - 1; i < x + 3; i++) {
            for (int j = y - 2; j < y + 2; j++) {
                if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.Stone) {
                    return true;
                }
            }
        }

        return false;
    }
}
