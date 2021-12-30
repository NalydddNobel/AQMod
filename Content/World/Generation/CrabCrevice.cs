using AQMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace AQMod.Content.World.Generation
{
    public static class CrabCrevice
    {


        public static void GenerateLegacyRavines(GenerationProgress progress)
        {
            progress.Message = Language.GetTextValue("Mods.AQMod.Common.OceanRavines");
            for (int i = 0; i < 5000; i++)
            {
                int x = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    x = Main.maxTilesX - x;
                for (int j = 200; j < Main.worldSurface; j++)
                {
                    if (CanPlaceLegacyOceanRavine(x, j))
                    {
                        int style = WorldGen.genRand.Next(3) + 3;
                        PlaceLegacyOceanRavine(x, j, style);
                        i += 1000;
                        break;
                    }
                }
            }
        }

        public static bool CanPlaceLegacyOceanRavine(int x, int y)
        {
            return !Framing.GetTileSafely(x, y).active() && Main.tile[x, y].liquid > 0 && Framing.GetTileSafely(x, y + 1).active() && Main.tileSand[Main.tile[x, y + 1].type];
        }

        public static void PlaceLegacyOceanRavine(int x, int y, int mossStyle = -1, int tileType = TileID.HardenedSand, int tileType2 = TileID.Sand, int wallType = 0)
        {
            if (wallType == 0)
                wallType = ModContent.WallType<Walls.OceanRavineWall>();
            int height = WorldGen.genRand.Next(40, 120);
            int digDir = x < Main.maxTilesX / 2 ? 1 : -1;
            int[] xAdds = new int[height];
            int x3 = x;
            int x2 = x;
            for (int i = 0; i < height; i++)
            {
                if (WorldGen.genRand.NextBool())
                {
                    xAdds[i] = digDir;
                    x += xAdds[i];
                }
                for (int j = 0; j < 20; j++)
                {
                    WorldGen.PlaceTile(x - 10 + j, y + i, tileType, true, true);
                    Framing.GetTileSafely(x - 10 + j, y + i).wall = (ushort)wallType;
                }
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(15, 25), 5, tileType, true);
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(20, 28), 5, tileType2, true);
            }
            for (int i = 0; i < 10; i++)
            {
                WorldGen.digTunnel(x2, y - i, digDir, 1f, 2, 6, true);
                for (int j = 0; j < 20; j++)
                {
                    Framing.GetTileSafely(x - 10 + j, y + height + i).wall = (ushort)wallType;
                }
            }
            for (int i = 0; i < height; i++)
            {
                x2 += xAdds[i];
                WorldGen.digTunnel(x2, y + i, digDir, 1f, 2, 6, true);
            }
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 28), 6, tileType, true);
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 33), 6, tileType2, true);
            WorldGen.digTunnel(x - digDir, y + height - 6, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 5, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 4, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 3, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 2, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 1, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height, digDir, 1f, 5, 6, true);
            if (mossStyle != -1)
            {
                int mossTile = ModContent.TileType<Tiles.CrabCrevice.ArgonMossSand>();
                if (mossStyle == 1)
                {
                    mossTile = ModContent.TileType<Tiles.CrabCrevice.KryptonMossSand>();
                }
                else if (mossStyle == 2)
                {
                    mossTile = ModContent.TileType<Tiles.CrabCrevice.XenonMossSand>();
                }
                for (int n = 0; n < height; n++)
                {
                    x3 += xAdds[n];
                    for (int m = -5; m < 20; m++)
                    {
                        GrassType.SpreadGrassToSurroundings(x3 + m, y + n, TileID.Sand, mossTile);
                        GrassType.SpreadGrassToSurroundings(x3 + m, y + n, TileID.HardenedSand, mossTile);
                        WorldGen.SquareTileFrame(x3 + m, y + n, true);
                    }
                    if (WorldGen.genRand.NextBool(5))
                    {
                        for (int m = 0; m < 15; m++)
                        {
                            if (WorldGen.genRand.NextBool())
                            {
                                if (!Framing.GetTileSafely(x3 + m, y + n).active() && Main.tile[x3 + m, y + n].liquid > 0)
                                {
                                    WorldGen.PlaceTile(x3 + m, y + n, ModContent.TileType<Torches>(), true, false, -1, mossStyle + 3);
                                    if (Framing.GetTileSafely(x3 + m, y + n).active() && Main.tile[x3 + m, y + n].type == ModContent.TileType<Torches>())
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void placeLegacyOceanRavine(int x, int y, int torchStyle = -1, int tileType = TileID.Sandstone, int tileType2 = TileID.HardenedSand, int wallType = 0)
        {
            if (wallType == 0)
                wallType = ModContent.WallType<Walls.OceanRavineWall>();
            int height = WorldGen.genRand.Next(40, 120);
            int digDir = x < Main.maxTilesX / 2 ? 1 : -1;
            int[] xAdds = new int[height];
            int x3 = x;
            int x2 = x;
            for (int i = 0; i < height; i++)
            {
                if (WorldGen.genRand.NextBool())
                {
                    xAdds[i] = digDir;
                    x += xAdds[i];
                }
                for (int j = 0; j < 20; j++)
                {
                    WorldGen.PlaceTile(x - 10 + j, y + i, tileType, true, true);
                    Framing.GetTileSafely(x - 10 + j, y + i).wall = (ushort)wallType;
                }
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(15, 25), 5, tileType, true);
                WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(20, 28), 5, tileType2, true);
            }
            for (int i = 0; i < 10; i++)
            {
                WorldGen.digTunnel(x2, y - i, digDir, 1f, 2, 6, true);
                for (int j = 0; j < 20; j++)
                {
                    Framing.GetTileSafely(x - 10 + j, y + height + i).wall = (ushort)wallType;
                }
            }
            for (int i = 0; i < height; i++)
            {
                x2 += xAdds[i];
                WorldGen.digTunnel(x2, y + i, digDir, 1f, 2, 6, true);
            }
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 28), 6, tileType, true);
            WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(20, 33), 6, tileType2, true);
            WorldGen.digTunnel(x - digDir, y + height - 6, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 5, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x - digDir, y + height - 4, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 3, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 2, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height - 1, digDir, 1f, 5, 6, true);
            WorldGen.digTunnel(x, y + height, digDir, 1f, 5, 6, true);
            if (torchStyle != -1)
            {
                for (int i = 0; i < height; i++)
                {
                    x3 += xAdds[i];
                    if (WorldGen.genRand.NextBool(5))
                    {
                        for (int j = 0; j < 15; j++)
                        {
                            if (WorldGen.genRand.NextBool())
                            {
                                if (!Framing.GetTileSafely(x3 + j, y + i).active() && Main.tile[x3 + j, y + i].liquid > 0)
                                {
                                    WorldGen.PlaceTile(x3 + j, y + i, ModContent.TileType<Torches>(), true, false, -1, torchStyle);
                                    if (Framing.GetTileSafely(x3 + j, y + i).active() && Main.tile[x3 + j, y + i].type == ModContent.TileType<Torches>())
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}