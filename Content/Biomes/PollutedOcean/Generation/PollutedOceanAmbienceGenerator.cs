using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Rubblemaker;
using Aequus.Common.WorldGeneration;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Ambient;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Ambient.Dripstones;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Ambient.Pots;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Ambient.SeaPickles;
using Aequus.Content.DataSets;
using Aequus.Content.Tiles.Furniture.Trash;
using System;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.PollutedOcean.Generation;
internal class PollutedOceanAmbienceGenerator : AequusGenStep {
    public override string InsertAfter => "Pots";

    protected override double GenWeight => 50f;

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

    public static bool Polluted(int x, int y) {
        if (WorldGen.SolidTile(x, y)) {
            return false;
        }

        for (int i = x; i < x + 1; i++) {
            for (int j = y - 1; j < y + 3; j++) {
                if (Main.tile[i, j].TileType == PollutedOceanGenerator._polymerSand || Main.tile[i, j].TileType == PollutedOceanGenerator._polymerSandstone || Main.tile[i, j].WallType == PollutedOceanGenerator._polymerSandstoneWall) {
                    return true;
                }
            }
        }

        return false;
    }

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
        _chest = (ushort)ModContent.TileType<TrashChest>();
        #endregion

        ChestsPlaced = 0;

        int left = PollutedOceanGenerator._LeftPadded;
        int right = PollutedOceanGenerator._RightPadded;
        int top = Math.Max(PollutedOceanGenerator.Y - 25, 10);
        int bottom = Main.UnderworldLayer + 20;
        SetMessage(progress);
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                SetProgress(progress, RectangleProgress(i, j, left, right, top, bottom), 0f, 0.5f);

                if (Random.NextBool(3000)) {
                    if (!Polluted(i, j)) {
                        continue;
                    }
                    PlaceSeaPickleSetPiece(i, j, Random.Next(30, 50));
                }
            }
        }

        bool wantChest = false;
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                SetProgress(progress, RectangleProgress(i, j, left, right, top, bottom), 0.5f, 1f);

                var tile = Main.tile[i, j];
                if (tile.TileType == TileID.Pots) {
                    if (!Polluted(i, j)) {
                        continue;
                    }

                    WorldGen.KillTile(i, j);
                }
                else if (WorldGen.SolidTile(i, j + 1)) {
                    if (wantChest || Random.NextBool(120)) {
                        if (!Polluted(i, j)) {
                            continue;
                        }

                        if (PlaceChest(i, j)) {
                            ChestsPlaced++;
                            wantChest = false;
                        }
                        else {
                            wantChest = true;
                        }
                    }
                    if (Random.NextBool(3)) {
                        if (!Polluted(i, j)) {
                            continue;
                        }

                        PlacePot(i, j);
                    }
                    else if (Random.NextBool()) {
                        if (!Polluted(i, j)) {
                            continue;
                        }

                        PlaceAmbientTile(i, j);
                    }
                    else if (Random.NextBool(3)) {
                        if (!Polluted(i, j)) {
                            continue;
                        }

                        PlaceStalagmite(i, j);
                    }
                }
                else if (WorldGen.SolidTile(i, j - 1)) {
                    if (Random.NextBool(3)) {
                        if (!Polluted(i, j)) {
                            continue;
                        }

                        if (Random.NextBool()) {
                            WorldGen.PlaceTile(i, j, _stalactite1x1, style: Random.Next(3));
                        }
                        else {
                            WorldGen.PlaceTile(i, j, _stalactite1x2, style: Random.Next(3));
                        }
                    }
                }
            }
        }
    }

    private static bool PlaceChest(int i, int j) {
        int chestId = WorldGen.PlaceChest(i, j, TileID.Containers, notNearOtherChests: true, style: ChestType.TrashCan);
        if (chestId == -1) {
            return false;
        }

        Chest chest = Main.chest[chestId];
        LootDefinition primaryLoot = Loot.PollutedOceanPrimary[ChestsPlaced % Loot.PollutedOceanPrimary.Count];
        chest.AddItemLoot(primaryLoot);
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
                if (WorldGen.InWorld(x, y, 10) && PollutedOceanGenerator.ReplaceableWall[Main.tile[x, y].WallType] && Main.tile[x, y].LiquidAmount == 255 && WorldGen.SolidTile(x, y + 1) && !Random.NextBool(4)) {
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
