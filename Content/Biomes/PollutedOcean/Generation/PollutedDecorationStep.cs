#if POLLUTED_OCEAN
using Aequus.Common.ContentTemplates.Tiles.Rubblemaker;
using Aequus.Content.Chests;
using Aequus.Content.Tiles.PollutedOcean.Ambient;
using Aequus.Content.Tiles.PollutedOcean.Ambient.Dripstones;
using Aequus.Content.Tiles.PollutedOcean.Ambient.Pots;
using Aequus.Content.Tiles.PollutedOcean.Ambient.SeaPickles;
using Aequus.Systems.Chests;
using Aequus.Systems.Chests.DropRules;
using Aequus.Systems.WorldGeneration;
using System;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.PollutedOcean.Generation;
internal class PollutedDecorationStep : AGenStep {
    public override string InsertAfter => "Pots";

    protected override double Weight => 50f;

    public static int ChestsPlaced;

    #region Tile Id Caches
    // Cache the tile ids to prevent spam usage of ModContent.TileType<T>().
    private static ushort _pot1x1;
    private static ushort _pot2x2;
    private static ushort _ambient1x1;
    private static ushort _ambient2x2;
    private static ushort _stalactite1x2;
    private static ushort _stalagmite1x2;
    private static ushort _stalactite1x1;
    private static ushort _stalagmite1x1;
    private static ushort _seaPickle1x1;
    private static ushort _seaPickle1x2;
    private static ushort _seaPickle2x2;
    private static ushort _chest;
    #endregion

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        #region Initialization
        _pot1x1 = (ushort)ModContent.TileType<TrashPots1x1>();
        _pot2x2 = (ushort)ModContent.TileType<TrashPots2x2>();
        _ambient1x1 = RubblemakerTile.GetId<PollutedOceanAmbient1x1>();
        _ambient2x2 = RubblemakerTile.GetId<PollutedOceanAmbient2x2>();
        _stalactite1x2 = (ushort)ModContent.TileType<PolymerStalactite1x2>();
        _stalagmite1x2 = (ushort)ModContent.TileType<PolymerStalagmite1x2>();
        _stalactite1x1 = (ushort)ModContent.TileType<PolymerStalactite1x1>();
        _stalagmite1x1 = (ushort)ModContent.TileType<PolymerStalagmite1x1>();
        _seaPickle1x1 = RubblemakerTile.GetId<SeaPickles1x1>();
        _seaPickle1x2 = RubblemakerTile.GetId<SeaPickles1x2>();
        _seaPickle2x2 = RubblemakerTile.GetId<SeaPickles2x2>();
#if POLLUTED_OCEAN_TODO
        _chest = (ushort)ModContent.TileType<TrashChest>();
#endif
        _chest = TileID.Containers2;
        #endregion

        ChestsPlaced = 0;

        var biome = Instance<PollutedBiomeStep>();
        var isPolluted = Instance<PollutedOceanSystem>().IsPolluted;

        int left = biome.LeftPadded;
        int right = biome.RightPadded;
        int top = Math.Max(biome.y - 25, 10);
        int bottom = Main.UnderworldLayer + 20;
        SetMessage(progress);
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                SetProgress(progress, RectangleProgress(i, j, left, right, top, bottom), 0f, 0.5f);

                Tile tile = Main.tile[i, j];
                if (!isPolluted.Contains(tile.TileType)) {
                    continue;
                }

                if (Random.NextBool(3000)) {
                    PlaceSeaPickleSetPiece(i, j, Random.Next(30, 50));
                }
            }
        }

        bool wantChest = false;
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                SetProgress(progress, RectangleProgress(i, j, left, right, top, bottom), 0.5f, 1f);

                Tile tile = Main.tile[i, j];
                if (!isPolluted.Contains(tile.TileType)) {
                    continue;
                }

                for (int k = i - 1; k <= i + 1; k++) {
                    for (int l = j - 1; l <= j + 1; l++) {
                        CheckTile(k, l, tile, ref wantChest);
                    }
                }
            }
        }
    }

    private static void CheckTile(int i, int j, Tile tile, ref bool wantChest) {
        if (Instance<PollutedOceanSystem>().RemoveFromGen.Contains(tile.TileType)) {
            WorldGen.KillTile(i, j);
        }

        if (WorldGen.SolidTile(i, j)) {
            return;
        }

        if (WorldGen.SolidTile(i, j + 1)) {
            if (wantChest || Random.NextBool(240)) {
                if (PlaceChest(i, j)) {
                    ChestsPlaced++;
                    wantChest = false;
                }
                else {
                    wantChest = true;
                }
            }
            else if (Random.NextBool(3)) {
                PlacePot(i, j);
            }
            else if (Random.NextBool()) {
                PlaceAmbientTile(i, j);
            }
            else if (Random.NextBool(3)) {
                PlaceStalagmite(i, j);
            }
            return;
        }

        if (WorldGen.SolidTile(i, j - 1)) {
            if (Random.NextBool(3)) {
                if (Random.NextBool()) {
                    WorldGen.PlaceTile(i, j, _stalactite1x1, style: Random.Next(3));
                }
                else {
                    WorldGen.PlaceTile(i, j, _stalactite1x2, style: Random.Next(3));
                }
            }
            return;
        }
    }

    private static bool PlaceChest(int i, int j) {
        int chestId = WorldGen.PlaceChest(i, j, TileID.Containers, notNearOtherChests: true, style: ChestID.TrashCan.Style);
        if (chestId == -1) {
            return false;
        }

        ChestLootInfo info = new(chestId, Random);
        ChestLootDatabase.Instance.SolveRules(ChestPool.PollutedOcean, in info);
        ChestsPlaced++;
        //if (Random.NextBool(5)) {
        //    chest.AddItem();
        //}
        return true;
    }

    private static void PlacePot(int i, int j) {
        if (Random.NextBool(4)) {
            WorldGen.PlaceTile(i, j, _pot1x1);
        }
        else {
            WorldGen.PlaceTile(i, j, _pot2x2, style: Random.Next(2));
        }
    }

    private static void PlaceAmbientTile(int i, int j) {
        if (Random.NextBool()) {
            WorldGen.PlaceTile(i, j, _ambient1x1);
        }
        else {
            WorldGen.PlaceTile(i, j, _ambient2x2);
        }
    }

    private static void PlaceStalagmite(int i, int j) {
        if (Random.NextBool()) {
            WorldGen.PlaceTile(i, j, _stalagmite1x1, style: Random.Next(3));
        }
        else {
            WorldGen.PlaceTile(i, j, _stalagmite1x2, style: Random.Next(3));
        }
    }

    private static void PlaceSeaPickleSetPiece(int i, int j, int size) {
        int startX = i - size;
        int endX = i + size;
        int progressX = 0;
        for (int x = startX; x < endX; x++) {
            int sizeY = (int)Math.Round(Math.Sin(progressX / (double)size * Math.PI) * size / 2.0);
            int endY = j + sizeY;
            progressX++;
            for (int y = j - sizeY; y < endY; y++) {
                if (WorldGen.InWorld(x, y, 10) && PollutedBiomeStep.ReplaceableWall[Main.tile[x, y].WallType] && Main.tile[x, y].LiquidAmount == 255 && WorldGen.SolidTile(x, y + 1) && !Random.NextBool(4)) {
                    ushort type = Random.Next(3) switch {
                        2 => _seaPickle1x1,
                        1 => _seaPickle1x2,
                        _ => _seaPickle2x2,
                    };
                    WorldGen.PlaceTile(x, y, type, style: Random.Next(3));
                }
            }
        }
    }
}
#endif