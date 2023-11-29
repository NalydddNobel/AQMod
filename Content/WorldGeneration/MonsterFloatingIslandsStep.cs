using Aequus.Common.WorldGeneration;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;
public class MonsterFloatingIslandsStep : AequusGenStep {
    public override string InsertAfter => "Floating Islands";

    public record struct IslandInfo(int X, int Y, int Style);

    public static readonly List<IslandInfo> IslandHouses = new();

    /// <summary>
    /// Generates an old-styled floating island.
    /// </summary>
    /// <param name="i">The X coordinate of the island.</param>
    /// <param name="j">The Y coordinate of the island.</param>
    /// <param name="tileType">Which tile type should be used to generate this island</param>
    /// <param name="hardTileType">A "Hard Block" for islands. Placed under <paramref name="tileType"></paramref> to ensure it doesn't fall.</param>
    /// <param name="wallType">The wall type.</param>
    public static void FloatingIsland(int i, int j, ushort tileType = TileID.Dirt, ushort? hardTileType = null, ushort wallType = WallID.DirtUnsafe) {
        int size = Random.Next(80, 120);
        int maxIterations = Random.Next(20, 25);
        var position = new Vector2(i, j);
        var velocity = default(Vector2);
        velocity.X = Random.Next(-20, 21) * 0.2f;
        while (velocity.X > -2f && velocity.X < 2f) {
            velocity.X = Random.Next(-20, 21) * 0.2f;
        }
        velocity.Y = Random.Next(-20, -10) * 0.02f;
        var hardBlock = hardTileType ?? tileType;
        while (size > 0 && maxIterations > 0) {
            size -= Random.Next(4);
            maxIterations--;
            int left = (int)(position.X - size * 0.5f);
            int right = (int)(position.X + size * 0.5f);
            int top = (int)(position.Y - size * 0.5f);
            int bottom = (int)(position.Y + size * 0.5f);
            if (left < 0) {
                left = 0;
            }
            if (right > Main.maxTilesX) {
                right = Main.maxTilesX;
            }
            if (top < 0) {
                top = 0;
            }
            if (bottom > Main.maxTilesY) {
                bottom = Main.maxTilesY;
            }
            float circleSize = size * Random.Next(80, 120) * 0.01f;
            float surfaceRandomness = position.Y + 1f;
            for (int k = left; k < right; k++) {
                if (Random.NextBool(2)) {
                    surfaceRandomness += Random.Next(-1, 2);
                }
                if (surfaceRandomness < position.Y) {
                    surfaceRandomness = position.Y;
                }
                if (surfaceRandomness > position.Y + 2f) {
                    surfaceRandomness = position.Y + 2f;
                }
                for (int l = (int)Math.Max(top, surfaceRandomness); l < bottom; l++) {
                    // Edited loop initializer start from the larger value so this check is uneeded.
                    //if (l < down) {
                    //    continue;
                    //}
                    float a = Math.Abs(k - position.X);
                    float b = Math.Abs(l - position.Y) * 2f;
                    float distance = (float)Math.Sqrt(a * a + b * b);
                    if (distance < circleSize * 0.4f) {
                        var tile = Main.tile[k, l];
                        tile.HasTile = true;
                        tile.TileType = tileType;
                    }
                }
            }
            WorldGen.TileRunner(Random.Next(left + 10, right - 10), (int)(position.Y + circleSize * 0.1f + 5f), Random.Next(5, 10), Random.Next(10, 15), hardBlock, addTile: true, 0f, 2f, noYChange: true);
            left = (int)(position.X - size * 0.4f);
            right = (int)(position.X + size * 0.4f);
            top = (int)(position.Y - size * 0.4f);
            bottom = (int)(position.Y + size * 0.4f);
            if (left < 0) {
                left = 0;
            }
            if (right > Main.maxTilesX) {
                right = Main.maxTilesX;
            }
            if (top < 0) {
                top = 0;
            }
            if (bottom > Main.maxTilesY) {
                bottom = Main.maxTilesY;
            }
            circleSize = size * Random.Next(80, 120) * 0.01f;
            for (int m = left; m < right; m++) {
                for (int n = top; n < bottom; n++) {
                    if (n > position.Y + 2f) {
                        float a = Math.Abs(m - position.X);
                        float b = Math.Abs(n - position.Y) * 2f;
                        float distance = (float)Math.Sqrt(a * a + b * b);
                        if (distance < circleSize * 0.4f) {
                            Main.tile[m, n].WallType = WallID.Dirt;
                        }
                    }
                }
            }
            position += velocity;
            velocity.Y += Random.Next(-10, 11) * 0.05f;
            if (velocity.X > 1f) {
                velocity.X = 1f;
            }
            else if (velocity.X < -1f) {
                velocity.X = -1f;
            }
            if (velocity.Y > 0.2f) {
                velocity.Y = -0.2f;
            }
            else if (velocity.Y < -0.2f) {
                velocity.Y = -0.2f;
            }
        }

        if (hardTileType == null) {
            return;
        }

        CheckFloatingBlocks(i, j, tileType, hardBlock);
    }

    public static void CheckFloatingBlocks(int i, int j, ushort softBlock, ushort hardBlock) {
        int left = Math.Max(i - 60, 0);
        int right = Math.Min(i + 60, Main.maxTilesX);
        int top = Math.Max(j - 60, 0);
        int bottom = Math.Min(j + 60, Main.maxTilesY - 1);
        for (int x = left; x < right; x++) {
            for (int y = top; y < bottom; y++) {
                var tile = Framing.GetTileSafely(x, y);
                if (tile.HasTile && tile.TileType == softBlock && (!Framing.GetTileSafely(x, y + 1).HasTile || !Framing.GetTileSafely(x, y + 2).HasTile)) {
                    tile.TileType = hardBlock;
                }
            }
        }
    }

    public static int GetIslandStyle(int x) {
        if (WorldGen.remixWorldGen && WorldGen.drunkWorldGen) {
            return (GenVars.crimsonLeft && x < Main.maxTilesX / 2) ? 5 : ((GenVars.crimsonLeft || x <= Main.maxTilesX / 2) ? 4 : 5);
        }
        else if (WorldGen.getGoodWorldGen || WorldGen.remixWorldGen) {
            return (!WorldGen.crimson) ? 4 : 5;
        }
        else if (Main.tenthAnniversaryWorld) {
            return 6;
        }
        return 0;
    }

    private static bool GetRandomCoordinates(out int x, out int y) {
        do {
            x = Random.Next((int)(Main.maxTilesX * 0.1), (int)(Main.maxTilesX * 0.9));
        }
        while (x > Main.maxTilesX / 2 - 150 && x < Main.maxTilesX / 2 + 150);

        y = 200;
        for (; y < Main.worldSurface; y++) {
            if (Main.tile[x, y].HasTile) {
                y = Random.Next(90, y - 100);
                return true;
            }
        }
        return false;
    }

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        IslandHouses.Clear();
        int islandCount = Main.maxTilesX / WorldGen.WorldSizeSmallX + 2;
        for (int i = 0; i < islandCount; i++) {
            for (int attempts = 0; attempts < 10000; attempts++) {
                if (!GetRandomCoordinates(out int randomX, out int randomY) || Main.tile[randomX, randomY].WallType > WallID.None || TileHelper.ScanTiles(new(randomX - 40, randomY - 20, 80, 40), TileHelper.HasTile)) {
                    continue;
                }

                var islandInfo = new IslandInfo(randomX, randomY, 0);
                if (!WorldGen.drunkWorldGen || WorldGen.remixWorldGen) {
                    islandInfo.Style = GetIslandStyle(randomX);
                    FloatingIsland(randomX, randomY);
                }
                else {
                    if (Random.NextBool(2)) {
                        islandInfo.Style = 3;
                        FloatingIsland(randomX, randomY, TileID.SnowBlock, TileID.IceBlock, WallID.IceUnsafe);
                    }
                    else {
                        islandInfo.Style = 1;
                        FloatingIsland(randomX, randomY, TileID.Sand, TileID.Sandstone, WallID.Sandstone);
                    }
                }

                IslandHouses.Add(islandInfo);
                break;
            }
        }
    }
}