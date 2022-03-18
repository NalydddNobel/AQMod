using AQMod.Tiles;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace AQMod.Content.World.Generation
{
    public sealed class GlobeGenerator : ModWorld
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int i = tasks.FindIndex((t) => t.Name.Equals("Micro Biomes"));
            if (i != -1)
            {
                tasks.Insert(i + 1, new PassLegacy("AQMod: Sky Globes", GenerateGlobes));
            }
        }

        public static void GenerateGlobes(GenerationProgress progress)
        {
            if (progress != null)
            {
                progress.Message = AQMod.GetText("Gen.SkyGlobes");
            }
            int globeCount = 0;
            int maxGlobes = Main.maxTilesX / 2100; // 2 in small, 4 in medium, 6 in large
            for (int i = 0; i < Main.maxTilesX && globeCount <= maxGlobes; i++)
            {
                int x = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
                int y = WorldGen.genRand.Next(50, (int)Main.worldSurface - 100);

                if (Main.tile[x, y] == null)
                {
                    Main.tile[x, y] = new Tile();
                    continue;
                }
                if (Main.tileTable[Main.tile[x, y].type])
                {
                    y--;
                    y -= Main.tile[x, y].frameY / 18;
                    if (Main.tile[x, y + 1] == null)
                    {
                        Main.tile[x, y + 1] = new Tile();
                        continue;
                    }
                    if (Main.tileTable[Main.tile[x, y + 1].type])
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            for (int l = 0; l < 2; l++)
                            {
                                var t = Main.tile[x + k, y - l];
                                if (t == null)
                                {
                                    Main.tile[x + k, y - l] = new Tile();
                                }
                                else if (t.active())
                                {
                                    if (t.Cuttable())
                                    {
                                        WorldGen.KillTile(x + k, y - l, noItem: true);
                                    }
                                    else
                                    {
                                        goto RepeatLoop;
                                    }
                                }
                            }
                        }
                        WorldGen.PlaceTile(x, y, ModContent.TileType<Globe>(), mute: true, forced: true);
                        if (Main.tile[x, y].type == ModContent.TileType<Globe>())
                        {
                            maxGlobes++;
                        }
                    }
                }
            RepeatLoop:
                continue;
            }
        }
    }
}