using Aequus.Common.WorldGeneration;
using Aequus.Content.Biomes.PollutedOcean.Tiles;
using Aequus.Content.Biomes.PollutedOcean.Tiles.Pots;
using Aequus.Content.Biomes.PollutedOcean.Tiles.SeaPickles;
using System;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.PollutedOcean;
internal class PollutedOceanAmbienceGenerator : AequusGenStep {
    public override string InsertAfter => "Pots";

    protected override double GenWeight => 50f;

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

    public static bool Polluted(int x, int y) {
        if (WorldGen.SolidTile(x, y)) {
            return false;
        }
        for (int i = x - 2; i < x + 3; i++) {
            for (int j = y - 2; j < y + 3; j++) {
                if (Main.tile[i, j].TileType == PollutedOceanGenerator._polymerSand || Main.tile[i, j].TileType == PollutedOceanGenerator._polymerSandstone || Main.tile[i, j].WallType == PollutedOceanGenerator._polymerSandstoneWall) {
                    return true;
                }
            }
        }

        return false;
    }

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        _pot1x1 = (ushort)ModContent.TileType<TrashPots1x1>();
        _pot2x2 = (ushort)ModContent.TileType<TrashPots2x2>();
        _ambient1x1 = (ushort)ModContent.TileType<PollutedOceanAmbient1x1>();
        _ambient2x2 = (ushort)ModContent.TileType<PollutedOceanAmbient2x2>();
        _stalactite1x2 = (ushort)ModContent.TileType<PolymerStalactite1x2>();
        _stalagmite1x2 = (ushort)ModContent.TileType<PolymerStalagmite1x2>();
        _stalactite1x1 = (ushort)ModContent.TileType<PolymerStalactite1x1>();
        _stalagmite1x1 = (ushort)ModContent.TileType<PolymerStalagmite1x1>();
        _seaPickle1x1 = (ushort)ModContent.TileType<SeaPickles1x1>();
        _seaPickle1x2 = (ushort)ModContent.TileType<SeaPickles1x2>();
        _seaPickle2x2 = (ushort)ModContent.TileType<SeaPickles2x2>();

        SetMessage(progress);
        for (int i = 10; i < Main.maxTilesX - 10; i++) {
            for (int j = 10; j < Main.maxTilesY - 10; j++) {
                if (!Polluted(i, j)) {
                    continue;
                }

                SetProgress(progress, IJLoopProgress(i,j), 0f, 0.5f);
                if (Random.NextBool(3000)) {
                    PlaceSeaPickleSetPiece(i, j, Random.Next(30, 50));
                }
            }
        }
        for (int i = 10; i < Main.maxTilesX - 10; i++) {
            for (int j = 10; j < Main.maxTilesY - 10; j++) {
                if (!Polluted(i, j)) {
                    continue;
                }

                SetProgress(progress, IJLoopProgress(i, j), 0.5f, 1f);
                var tile = Main.tile[i, j];
                if (tile.TileType == TileID.Pots) {
                    WorldGen.KillTile(i, j);
                }
                if (WorldGen.SolidTile(i, j + 1)) {
                    if (Random.NextBool(3)) {
                        if (Random.NextBool(4)) {
                            WorldGen.PlaceTile(i, j, _pot1x1);
                        }
                        else {
                            WorldGen.PlaceTile(i, j, _pot2x2, style: Random.Next(2));
                        }
                    }
                    else if (Random.NextBool()) {
                        if (Random.NextBool()) {
                            WorldGen.PlaceTile(i, j, _ambient1x1);
                        }
                        else {
                            WorldGen.PlaceTile(i, j, _ambient2x2);
                        }
                    }
                    else if (Random.NextBool(3)) {
                        if (Random.NextBool()) {
                            WorldGen.PlaceTile(i, j, _stalagmite1x1, style: Random.Next(3));
                        }
                        else {
                            WorldGen.PlaceTile(i, j, _stalagmite1x2, style: Random.Next(3));
                        }
                    }
                }
                else if (WorldGen.SolidTile(i, j - 1)) {
                    if (Random.NextBool(3)) {
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
