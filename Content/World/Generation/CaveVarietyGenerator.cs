using Aequus.Common.Preferences;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Content.World.Generation {
    public class CaveVarietyGenerator {
        public void WeirdCaves() {
            for (int i = 200; i < Main.maxTilesX - 200; i += 200) {
                for (int j = (int)Main.worldSurface + 100; j < Main.UnderworldLayer - 100; j += 200) {
                    if (!WorldGen.genRand.NextBool() && WorldGen.genRand.NextFloat(1f) <= GameplayConfig.Instance.CaveVariety)
                        continue;
                    GenerateWeirdCave(i, j);
                }
            }
        }
        public void GenerateWeirdCave(int i, int j) {
            for (int k = 0; k < 3; k++) {
                int size = WorldGen.genRand.Next(3, 8);
                int x = i;
                int y = j;
                var dir = new Vector2(WorldGen.genRand.NextFloat(-3f, 3f), WorldGen.genRand.NextFloat(-0.75f, 0.75f));
                if (WorldGen.genRand.NextBool(6) || Math.Abs(Main.maxTilesX - x * 2) < 400) {
                    for (; size > 0;) {
                        var v = WorldGen.digTunnel(x, y, dir.X, dir.Y, WorldGen.genRand.Next(5, 25), size);
                        x = (int)v.X;
                        y = (int)v.Y;
                        dir = dir.RotatedBy(WorldGen.genRand.NextFloat(-0.75f, 0.75f));
                        if (WorldGen.genRand.NextBool(10))
                            size--;
                    }
                }
                else {
                    for (; size > 0;) {
                        var v = WorldGen.digTunnel(x, y, dir.X, dir.Y, WorldGen.genRand.Next(5, 15), size);
                        x = (int)v.X;
                        y = (int)v.Y;
                        dir = dir.RotatedBy(WorldGen.genRand.NextFloat(-0.5f, 0.5f));
                        if (WorldGen.genRand.NextBool(3))
                            size--;
                    }
                }
            }
        }

        public void TallCaves() {
            for (int i = 200; i < Main.maxTilesX - 200; i += 200) {
                for (int j = (int)Main.worldSurface + 100; j < Main.UnderworldLayer - 100; j += 200) {
                    if (!WorldGen.genRand.NextBool(4) && WorldGen.genRand.NextFloat(1f) <= GameplayConfig.Instance.CaveVariety)
                        continue;

                    GenerateTallCave(i, j);
                }
            }
        }
        public void GenerateTallCave(int i, int j) {
            int Size = WorldGen.genRand.Next(8, 16);
            var Dir = new Vector2(WorldGen.genRand.NextFloat(-0.75f, 0.75f), WorldGen.genRand.NextFloat(-3f, -1f));
            int reps = 5;
            for (int k = -reps; k <= reps; k++) {
                int size = Size;
                int x = i + k * size / 2 + WorldGen.genRand.Next(-4, 4);
                int y = j + WorldGen.genRand.Next(-5, 8);
                var dir = new Vector2(Dir.X, Dir.Y * (1f - (1 + Math.Abs(k)) / (float)(1 + reps)) + Main.rand.NextFloat(-0.2f, 0.2f));
                if (WorldGen.genRand.NextBool(8) && Math.Abs(Main.maxTilesX - x * 2) > 400) {
                    for (; size > 0;) {
                        var v = WorldGen.digTunnel(x, y, dir.X, dir.Y, WorldGen.genRand.Next(4, 15), size);
                        x = (int)v.X;
                        y = (int)v.Y;
                        dir = dir.RotatedBy(WorldGen.genRand.NextFloat(-0.15f, 0.15f));
                        if (WorldGen.genRand.NextBool())
                            size--;
                        if (WorldGen.genRand.NextBool())
                            size--;
                    }
                }
                else {
                    for (; size > 0; size -= 2) {
                        var v = WorldGen.digTunnel(x, y, dir.X, dir.Y, WorldGen.genRand.Next(4, 10), size);
                        x = (int)v.X;
                        y = (int)v.Y;
                        dir = dir.RotatedBy(WorldGen.genRand.NextFloat(-0.05f, 0.05f));
                    }
                }
            }
        }
    }
}