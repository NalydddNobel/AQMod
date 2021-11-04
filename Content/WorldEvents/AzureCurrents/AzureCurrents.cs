using System;
using Terraria;
using Terraria.ID;

namespace AQMod.Content.WorldEvents.AzureCurrents
{
    public sealed class AzureCurrents
    {
        public bool IsActive { get; private set; }
        public bool EventActive(Player player)
        {
            return IsActive && InSpace(player);
        }
        public static bool MeteorTime()
        {
            if (Main.time < 14400)
                return true;
            if (Main.dayTime)
            {
                return Main.time > Main.dayLength - 14400;
            }
            return Main.time > Main.nightLength - 14400;
        }

        public static bool InSpace(Player player)
        {
            return InSpace(player.position.Y);
        }

        public static bool InSpace(float y)
        {
            return y < 3000f; // 187.5 tiles
        }

        internal void Reset()
        {
            IsActive = false;
        }

        internal void UpdateWorld()
        {
            IsActive = ImitatedWindyDay.IsItAHappyWindyDay;
        }

        public static bool CanCrashMeteor(int x, int y, int size = 40)
        {
            int minX = x - size / 2;
            int maxX = x + size / 2;
            int minY = y - size / 2;
            int maxY = y + size / 2;
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    if (Main.tileContainer[Main.tile[i, j].type])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static void CrashMeteor_Orig(int x, int y, int size, bool doEffects = true, ushort tileType = TileID.Meteorite)
        {
            // doesn't do reflection for no-drops since it might as well drop tiles.
            int num = WorldGen.genRand.Next(17, 23);
            for (int i = x - num; i < x + num; i++)
            {
                for (int j = y - num; j < y + num; j++)
                {
                    if (j <= y + Main.rand.Next(-2, 3) - 5)
                    {
                        continue;
                    }
                    float iX = Math.Abs(x - i);
                    float iY = Math.Abs(y - j);
                    if ((float)Math.Sqrt(iX * iX + iY * iY) < num * 0.9 + Main.rand.Next(-4, 5))
                    {
                        int t = Main.tile[i, j].type;
                        if (t != tileType)
                            WorldGen.KillTile(i, j);
                        if (Main.tileSolid[t])
                            Main.tile[i, j].active(active: true);
                        Main.tile[i, j].type = tileType;
                    }
                }
            }
            num = WorldGen.genRand.Next(8, 14);
            for (int i = x - num; i < x + num; i++)
            {
                for (int j = y - num; j < y + num; j++)
                {
                    if (j > y + Main.rand.Next(-2, 3) - 4)
                    {
                        float iX = Math.Abs(x - i);
                        float iY = Math.Abs(y - j);
                        if ((float)Math.Sqrt(iX * iX + iY * iY) < num * 0.8 + Main.rand.Next(-3, 4))
                        {
                            if (Main.tile[i, j].type != tileType)
                                WorldGen.KillTile(i, j, fail: false, effectOnly: false, noItem: false);
                        }
                    }
                }
            }
            num = WorldGen.genRand.Next(25, 35);
            for (int i = x - num; i < x + num; i++)
            {
                for (int j = y - num; j < y + num; j++)
                {
                    float iX = Math.Abs(x - i);
                    float iY = Math.Abs(y - j);
                    if ((float)Math.Sqrt(iX * iX + iY * iY) < num * 0.7)
                    {
                        WorldGen.KillTile(i, j);
                        Main.tile[i, j].liquid = 0;
                    }
                    if (Main.tile[i, j].type == tileType)
                    {
                        if (!WorldGen.SolidTile(i - 1, j) &&
                            !WorldGen.SolidTile(i + 1, j) &&
                            !WorldGen.SolidTile(i, j - 1) &&
                            !WorldGen.SolidTile(i, j + 1))
                        {
                            Main.tile[i, j].active(active: false);
                        }
                        else if ((Main.tile[i, j].halfBrick() || Main.tile[i - 1, j].topSlope()) &&
                            !WorldGen.SolidTile(i, j + 1))
                        {
                            WorldGen.KillTile(i, j);
                        }
                    }
                    WorldGen.SquareTileFrame(i, j);
                    WorldGen.SquareWallFrame(i, j);
                }
            }
            num = WorldGen.genRand.Next(23, 32);
            for (int i = x - num; i < x + num; i++)
            {
                for (int j = y - num; j < y + num; j++)
                {
                    if (j <= y + WorldGen.genRand.Next(-3, 4) - 3 || !Main.tile[i, j].active() || Main.rand.Next(10) != 0)
                    {
                        continue;
                    }
                    float iX = Math.Abs(x - i);
                    float iY = Math.Abs(y - j);
                    if ((float)Math.Sqrt(iX * iX + iY * iY) < num * 0.8)
                    {
                        int t = Main.tile[i, j].type;
                        if (t != tileType)
                            WorldGen.KillTile(i, j);
                        if (Main.tileSolid[t])
                            Main.tile[i, j].active(active: true);
                        Main.tile[i, j].type = tileType;
                        WorldGen.SquareTileFrame(i, j);
                    }
                }
            }
            num = WorldGen.genRand.Next(30, 38);
            for (int i = x - num; i < x + num; i++)
            {
                for (int j = y - num; j < y + num; j++)
                {
                    if (j <= y + WorldGen.genRand.Next(-2, 3) || !Main.tile[i, j].active() || Main.rand.Next(20) != 0)
                    {
                        continue;
                    }
                    float iX = Math.Abs(x - i);
                    float iY = Math.Abs(y - j);
                    if ((float)Math.Sqrt(iX * iX + iY * iY) < num * 0.85)
                    {
                        int t = Main.tile[i, j].type;
                        if (t != tileType)
                            WorldGen.KillTile(i, j);
                        if (Main.tileSolid[t])
                            Main.tile[i, j].active(active: true);
                        Main.tile[i, j].type = tileType;
                        WorldGen.SquareTileFrame(i, j);
                    }
                }
            }

            AQMod.BroadcastMessage(Lang.gen[59].Key, AQMod.EventMessage);
            if (Main.netMode != 1)
            {
                NetMessage.SendTileSquare(-1, x, y, 40);
            }
        }
    }
}