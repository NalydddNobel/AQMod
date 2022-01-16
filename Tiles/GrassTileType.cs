using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Tiles
{
    public abstract class GrassTileType : ModTile
    {
        public abstract int? OriginalTile { get; }

        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            TileID.Sets.Grass[Type] = true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (OriginalTile != null && !effectOnly && fail)
            {
                Main.tile[i, j].type = (ushort)OriginalTile.Value;
            }
        }

        public static void SpreadGrassToSurroundings(int i, int j, int dirt, int grass, int spread = 0, byte color = 0)
        {
            int minX = i - 1;
            int maxX = i + 2;
            int minY = j - 1;
            int maxY = j + 2;
            if (minX < 10)
            {
                minX = 10;
            }
            if (maxX > Main.maxTilesX - 10)
            {
                maxX = Main.maxTilesX - 10;
            }
            if (minY < 10)
            {
                minY = 10;
            }
            if (maxY > Main.maxTilesY - 10)
            {
                maxY = Main.maxTilesY - 10;
            }
            bool flag33 = false;
            for (int k = minX; k < maxX; k++)
            {
                for (int l = minY; l < maxY; l++)
                {
                    if ((i != k || j != l) && Main.tile[k, l].active() && Main.tile[k, l].type == dirt)
                    {
                        SpreadGrassToTile(k, l, dirt, grass, spread, color);
                        if (Main.tile[k, l].type == grass)
                        {
                            WorldGen.SquareTileFrame(k, l);
                            flag33 = true;
                        }
                    }
                }
            }
            if (Main.netMode == 2 && flag33)
            {
                NetMessage.SendTileSquare(-1, i, j, 3);
            }
        }

        public static void SpreadGrassToTile(int i, int j, int dirt, int grass, int spread = 0, byte color = 0)
        {
            try
            {
                if (WorldGen.InWorld(i, j, 1) && Main.tile[i, j].type == dirt && Main.tile[i, j].active())
                {
                    int minX = i - 1;
                    int maxX = i + 2;
                    int minY = j - 1;
                    int maxY = j + 2;
                    if (minX < 0)
                    {
                        minX = 0;
                    }
                    if (maxX > Main.maxTilesX)
                    {
                        maxX = Main.maxTilesX;
                    }
                    if (minY < 0)
                    {
                        minY = 0;
                    }
                    if (maxY > Main.maxTilesY)
                    {
                        maxY = Main.maxTilesY;
                    }
                    bool canSpread = true;
                    for (int k = minX; k < maxX; k++)
                    {
                        for (int l = minY; l < maxY; l++)
                        {
                            if (!Main.tile[k, l].active() || !Main.tileSolid[Main.tile[k, l].type])
                            {
                                canSpread = false;
                            }
                            if (Main.tile[k, l].lava() && Main.tile[k, l].liquid > 0)
                            {
                                canSpread = true;
                                break;
                            }
                        }
                    }
                    if (!canSpread)
                    {
                        Main.tile[i, j].type = (ushort)grass;
                        Main.tile[i, j].color(color);

                        for (int m = minX; m < maxX; m++)
                        {
                            for (int l = minY; l < maxY; l++)
                            {
                                if (Main.tile[m, l].active() && Main.tile[m, l].type == dirt)
                                {
                                    try
                                    {
                                        if (spread > 0)
                                        {
                                            SpreadGrassToTile(m, l, dirt, grass, spread - 1, color);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}