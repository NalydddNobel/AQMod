using AQMod.Assets;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Content.World.Events
{
    public sealed class GaleStreams : WorldEvent
    {
        public sealed class CustomProgressBar : EventProgressBar
        {
            public override Texture2D IconTexture => ModContent.GetTexture(TexturePaths.EventIcons + "galestreams");
            public override string EventName => Language.GetTextValue("Mods.AQMod.GaleStreams");
            public override Color NameBGColor => new Color(20, 90 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10), 90 + (int)(Math.Sin(Main.GlobalTime * 5f) * 10 + MathHelper.Pi), 128);
            public override float EventProgress => (int)(Main.windSpeed * 100).Abs() / 300f;

            public override bool IsActive() => EventProgressBarLoader.ShouldShowGaleStreamsProgressBar && GaleStreams.EventActive(Main.LocalPlayer) && WorldDefeats.SudoHardmode;
            public override string ModifyProgressText(string text) => Language.GetTextValue("Mods.AQMod.EventProgress.GaleStreams", (int)(Main.windSpeed * 100).Abs(), 300);
        }

        internal override EventProgressBar ProgressBar => new CustomProgressBar();

        public static Color HotCurrentColor => new Color(43, 148, 240, 255);
        public static Color ColdCurrentColor => new Color(255, 94, 31, 255);
        public static Color NeutralCurrentColor => new Color(255, 255, 255, 255);

        public static bool IsActive { get; private set; }
        public static bool EndEvent;
        public static bool EventActive(Player player)
        {
            return IsActive && InSpace(player);
        }

        public static void ProgressEvent(Player player, int points)
        {
            if (!WorldDefeats.SudoHardmode || player.dead || !player.active || EndEvent)
                return;
            Main.windSpeedSet += Math.Sign(Main.windSpeedSet) * points / 100f;
            if (Main.windSpeedSet >= 3f)
            {
                WorldDefeats.DownedGaleStreams = true;
                if (Main.netMode == NetmodeID.Server)
                    NetHelper.UpdateWindSpeeds();
                Main.windSpeedSet = 3f;
                EndEvent = true;
            }
            else if (Main.windSpeedSet <= -3f)
            {
                WorldDefeats.DownedGaleStreams = true;
                if (Main.netMode == NetmodeID.Server)
                    NetHelper.UpdateWindSpeeds();
                Main.windSpeedSet = -3f;
                EndEvent = true;
            }
            Main.windSpeedTemp = Main.windSpeedSet;
        }

        public static bool MeteorTime()
        {
            if (Main.time < 3600)
                return true;
            if (Main.dayTime)
                return Main.time > Main.dayLength - 3600;
            return Main.time > Main.nightLength - 3600;
        }

        public static bool InSpace(Player player)
        {
            return InSpace(player.position.Y);
        }

        public static bool InSpace(float y)
        {
            // the magic number "2.66666666..." comes from divding by 6, 
            // 1200 / 6 = 200 -- small world size
            // then converting that into world coordinates, by multiplying by 16
            // 1200 / 6 * 16 = 3200
            // you can then find a number to multiply by in order to get the same result as dividing by 6 and multiplying by 16
            // 3200 / 1200 = 2.6666-....
            return y < Main.maxTilesY * 2.66666666f; // 200 tiles in small, 300 in medium, 400 in large
        }

        public override TagCompound Save()
        {
            return new TagCompound()
            {
                ["EndEvent"] = EndEvent,
            };
        }

        public override void Load(TagCompound tag)
        {
            EndEvent = tag.GetBool("EndEvent");
        }

        public override void PostUpdate()
        {
            IsActive = ImitatedWindyDay.IsItAHappyWindyDay_WindyEnough;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(IsActive);
        }

        public override void NetReceive(BinaryReader reader)
        {
            IsActive = reader.ReadBoolean();
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
                        return false;
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
                                Main.tile[i, j].active(active: true);
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
                        int scatterX = Main.rand.NextVRand(-scatter, scatter);
                        int scatterY = Main.rand.NextVRand(-scatter, scatter);
                        bool active = Main.tile[i + scatterX, j + scatterY].active();
                        int type = Main.tile[i + scatterX, j + scatterY].type;
                        if (type != tileType)
                        {
                            WorldGen.KillTile(i + scatterX, j + scatterY, fail: false, effectOnly: false, noItem: false);
                            if (active && Main.tileSolid[type])
                                Main.tile[i + scatterX, j + scatterY].active(active: true);
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
                        Main.tile[i, j].active(active: false);
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
                            int scatterX = Main.rand.NextVRand(-1, 1);
                            int scatterY = Main.rand.NextVRand(-1, 1);
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
                            WorldGen.SquareTileFrame(i, j, true);
                    }
                }
            }

            AQMod.BroadcastMessage(Lang.gen[59].Key, Coloring.EventMessage);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NetMessage.SendTileSquare(-1, minX, minY, size);
        }
    }
}
