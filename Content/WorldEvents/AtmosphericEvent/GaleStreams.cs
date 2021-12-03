using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Content.WorldEvents.AtmosphericEvent
{
    public sealed class GaleStreams : ModWorld
    {
        public static Color HotCurrentColor => new Color(43, 148, 240, 255);
        public static Color ColdCurrentColor => new Color(255, 94, 31, 255);
        public static Color NeutralCurrentColor => new Color(255, 255, 255, 255);

        public static bool IsActive { get; private set; }
        public static bool EventActive(Player player)
        {
            return IsActive && InSpace(player);
        }
        public static bool MeteorTime()
        {
            if (Main.time < 3600)
                return true;
            if (Main.dayTime)
            {
                return Main.time > Main.dayLength - 3600;
            }
            return Main.time > Main.nightLength - 3600;
        }

        public static bool InMeteorSpawnZone(float y)
        {
            return y < 2000f;
        }

        public static bool InSpace(Player player)
        {
            return InSpace(player.position.Y);
        }

        public static bool InSpace(float y)
        {
            return y < 3000f; // 187.5 tiles
        }

        internal static void Reset()
        {
            IsActive = false;
        }

        public override void PostUpdate()
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

        public static void CrashMeteor(int x, int y, int size = 40, int scatter = 1, int scatterAmount = 4, int scatterChance = 25, int holeSizeDivider = 3, bool doEffects = true, ushort tileType = TileID.Meteorite)
        {
            int circularSize = size - 8 - scatter;
            int halfSize = circularSize / 2;
            int minX = x - halfSize;
            int maxX = x + halfSize;
            int minY = y - halfSize;
            int maxY = y + halfSize;
            // draws the circle of the meteorite
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        bool active = Main.tile[i, j].active();
                        int type = Main.tile[i, j].type;
                        if (active && type != tileType)
                        {
                            WorldGen.KillTile(i, j, fail: false, effectOnly: false, noItem: false);
                            if (Main.tileSolid[type])
                            {
                                Main.tile[i, j].active(active: true);
                            }
                        }
                        Main.tile[i, j].type = tileType;
                    }
                }
            }

            halfSize = size / 2 - scatter;
            minX = x - halfSize;
            maxX = x + halfSize;
            minY = y - halfSize;
            maxY = y + halfSize;

            // does some scatter on the outside
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize && WorldGen.genRand.NextBool(25))
                    {
                        int scatterX = AQUtils.NextVRand(Main.rand, -scatter, scatter);
                        int scatterY = AQUtils.NextVRand(Main.rand, -scatter, scatter);
                        bool active = Main.tile[i + scatterX, j + scatterY].active();
                        int type = Main.tile[i + scatterX, j + scatterY].type;
                        if (type != tileType)
                        {
                            WorldGen.KillTile(i + scatterX, j + scatterY, fail: false, effectOnly: false, noItem: false);
                            if (active && Main.tileSolid[type])
                            {
                                Main.tile[i + scatterX, j + scatterY].active(active: true);
                            }
                        }
                        Main.tile[i + scatterX, j + scatterY].type = tileType;
                    }
                }
            }

            circularSize = size / holeSizeDivider;
            halfSize = circularSize / 2;
            minX = x - circularSize;
            maxX = x + circularSize;
            minY = y - circularSize;
            maxY = y + circularSize;

            // carves a hole in the middle
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        Main.tile[i, j].active(active: false);
                    }
                }
            }

            // does some scatter in the center of the meteorite
            for (int k = 0; k < scatterAmount; k++)
            {
                for (int i = minX; i < maxX; i++)
                {
                    for (int j = minY; j < maxY; j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                            continue;
                        }
                        int iX = i - x;
                        int iY = j - y;
                        int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                        if (distance < halfSize)
                        {
                            int scatterX = AQUtils.NextVRand(Main.rand, -1, 1);
                            int scatterY = AQUtils.NextVRand(Main.rand, -1, 1);
                            Main.tile[i + scatterX, j + scatterY].active(active: false);
                        }
                    }
                }
            }

            halfSize = size / 2;
            minX = x - halfSize;
            maxX = x + halfSize;
            minY = y - halfSize;
            maxY = y + halfSize;
            // runs square tile frame on everything here
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        if (Main.tile[i, j].active() && Main.tile[i, j].type == tileType)
                        {
                            WorldGen.SquareTileFrame(i, j, true);
                        }
                    }
                }
            }

            AQMod.BroadcastMessage(Lang.gen[59].Key, AQMod.EventMessage);
            if (Main.netMode != 1)
            {
                NetMessage.SendTileSquare(-1, minX, minY, size);
            }
        }
    }
}