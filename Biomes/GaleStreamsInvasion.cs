using Aequus.Sounds;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes
{
    public class GaleStreamsInvasion : ModBiome
    {
        public static InvasionStatus Status { get; set; }
        public static byte updateTimer;
        public static bool SupressWindUpdates { get; set; }

        public override int Music => MusicData.GaleStreamsEvent.GetID();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override string BestiaryIcon => Aequus.AssetsPath + "UI/BestiaryIcons/GaleStreams";

        public override bool IsBiomeActive(Player player)
        {
            return player.Aequus().eventGaleStreams;
        }

        public static bool IsThisSpace(Player player)
        {
            return IsThisSpace(player.position.Y);
        }
        public static bool IsThisSpace(float y)
        {
            // the magic number "2.66666666..." comes from divding by 6, 
            // 1200 / 6 = 200 -- small world size
            // then converting that into world coordinates, by multiplying by 16
            // 1200 / 6 * 16 = 3200
            // you can then find a number to multiply by in order to get the same result as dividing by 6 and multiplying by 16
            // 3200 / 1200 = 2.6666-....
            return y < Main.maxTilesY * 2.66666666f; // 200 tiles in small, 300 in medium, 400 in large
        }

        public static bool TimeForMeteorSpawns()
        {
            if (Main.time < 3600)
                return true;
            if (Main.dayTime)
                return Main.time > Main.dayLength - 3600;
            return Main.time > Main.nightLength - 3600;
        }

        public static bool MeteorCheck(int x, int y, int size = 40)
        {
            int minX = x - size / 2;
            int maxX = x + size / 2;
            int minY = y - size / 2;
            int maxY = y + size / 2;
            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (Main.tileContainer[Main.tile[i, j].TileType])
                        return false;
                }
            }
            return true;
        }

        public static bool CrashMeteor(int x, int y, int size = 40, int scatter = 1, int scatterAmount = 4, int scatterChance = 25, int holeSizeDivider = 3, bool doEffects = true, bool checkIfCan = true, ushort tileType = TileID.Meteorite)
        {
            if (checkIfCan && !MeteorCheck(x, y, 24))
            {
                return false;
            }
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
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        bool active = Main.tile[i, j].HasTile;
                        int type = Main.tile[i, j].TileType;
                        if (active && type != tileType)
                        {
                            WorldGen.KillTile(i, j, fail: false, effectOnly: false, noItem: false);
                            if (Main.tileSolid[type])
                            {
                                Main.tile[i, j].Active(true);
                            }
                        }
                        Main.tile[i, j].TileType = tileType;
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
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize && WorldGen.genRand.NextBool(25))
                    {
                        int scatterX = Main.rand.Next(-scatter, scatter);
                        int scatterY = Main.rand.Next(-scatter, scatter);
                        bool active = Main.tile[i + scatterX, j + scatterY].HasTile;
                        int type = Main.tile[i + scatterX, j + scatterY].TileType;
                        if (type != tileType)
                        {
                            WorldGen.KillTile(i + scatterX, j + scatterY, fail: false, effectOnly: false, noItem: false);
                            if (active && Main.tileSolid[type])
                            {
                                Main.tile[i + scatterX, j + scatterY].Active(true);
                            }
                        }
                        Main.tile[i + scatterX, j + scatterY].TileType = tileType;
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
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        Main.tile[i, j].Active(false);
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
                        int iX = i - x;
                        int iY = j - y;
                        int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                        if (distance < halfSize)
                        {
                            int scatterX = Main.rand.Next(3) - 1;
                            int scatterY = Main.rand.Next(3) - 1;
                            Main.tile[i + scatterX, j + scatterY].Active(false);
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
                    int iX = i - x;
                    int iY = j - y;
                    int distance = (int)Math.Sqrt(iX * iX + iY * iY);
                    if (distance < halfSize)
                    {
                        if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                            WorldGen.SquareTileFrame(i, j, true);
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
                NetMessage.SendTileSquare(-1, minX, minY, size);
            return true;
        }

        public static bool CheckActive(Player player)
        {
            return Status == InvasionStatus.Active && IsThisSpace(player.position.Y * 1.5f)
                && player.townNPCs < 1f && !player.ZonePeaceCandle && !player.behindBackWall;
        }

        public class GaleStreamsSystem : ModSystem
        {
            public override void PreUpdateEntities()
            {
                if (!Aequus.HardmodeTier)
                {
                    Status = InvasionStatus.Inactive;
                    return;
                }
                if (Main.WindyEnoughForKiteDrops)
                {
                    Status = InvasionStatus.Active;
                }
                else
                {
                    Status = InvasionStatus.Inactive;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Status == InvasionStatus.Active)
                    {
                        InnerUpdateActive();
                    }
                }
            }
            public void InnerUpdateActive()
            {
                if (updateTimer == 1)
                {
                    SupressWindUpdates = false;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && Main.player[i].Aequus().eventGaleStreams)
                        {
                            SupressWindUpdates = true;
                            break;
                        }
                    }
                }
                updateTimer++;

                if (SupressWindUpdates)
                {
                    Main.windCounter = Math.Max(Main.windCounter, 360);
                }
            }
        }
    }
}