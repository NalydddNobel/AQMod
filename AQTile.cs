using AQMod.Common;
using AQMod.Tiles;
using AQMod.Tiles.CrabCrevice;
using AQMod.Tiles.ExporterQuest;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQTile : GlobalTile
    {
        internal const byte Slope_BottomLeft = 1;
        internal const byte Slope_BottomRight = 2;
        internal const byte Slope_TopLeft = 3;
        internal const byte Slope_TopRight = 4;

        public sealed class Sets
        {
            public static Sets Instance;

            public HashSet<ushort> CanFixWaterOnType { get; private set; }
            public HashSet<ushort> ExporterQuestFurniture { get; private set; }

            public Sets()
            {
                CanFixWaterOnType = new HashSet<ushort>()
                {
                    TileID.Grass,
                    TileID.CorruptGrass,
                    TileID.FleshGrass,
                    TileID.SnowBlock,
                    TileID.IceBlock,
                    TileID.Sand,
                };

                ExporterQuestFurniture = new HashSet<ushort>()
                {
                    (ushort)ModContent.TileType<JeweledChandlierTile>(),
                    (ushort)ModContent.TileType<JeweledCandelabraTile>(),
                    (ushort)ModContent.TileType<JeweledChaliceTile>(),
                };
            }
        }

        private bool TryPlaceHerb(int i, int j, int[] validTile, int style)
        {
            for (int y = j - 1; y > 20; y--)
            {
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                    continue;
                }
                if (!Main.tile[i, y].active() && Main.tile[i, y + 1].active())
                {
                    for (int k = 0; k < validTile.Length; k++)
                    {
                        if (Main.tile[i, y + 1].type == validTile[k] && CheckForType(new Rectangle(i - 6, y - 6, 12, 12).KeepInWorld(20), ModContent.TileType<Herbs>()))
                        {
                            WorldGen.PlaceTile(i, y, ModContent.TileType<Herbs>(), mute: true, forced: true, style: style);
                            return Framing.GetTileSafely(i, y).type == ModContent.TileType<Herbs>();
                        }
                    }
                }
            }
            return false;
        }
        public override void RandomUpdate(int i, int j, int type)
        {
            if (Main.tile[i, j] == null)
                Main.tile[i, j] = new Tile();
            if (!WorldDefeats.DownedDemonSiege && WorldGen.genRand.NextBool(10000) && GoreNest.TryGrowGoreNest(i, j, true, true))
            {
                return;
            }
            switch (type)
            {
                case TileID.Cloud:
                case TileID.RainCloud:
                case TileID.SnowCloud:
                    if (WorldDefeats.DownedGaleStreams && j < Main.rockLayer && WorldGen.genRand.NextBool(10))
                    {
                        TryPlaceHerb(i, j, new int[] { TileID.Cloud, TileID.RainCloud, TileID.SnowCloud, }, 1);
                    }
                    break;

                case TileID.Meteorite:
                    if (WorldDefeats.DownedStarite && j < Main.rockLayer && WorldGen.genRand.NextBool(10))
                    {
                        TryPlaceHerb(i, j, new int[] { TileID.Meteorite, }, 0);
                    }
                    break;

                case TileID.Stone:
                    if (j > Main.rockLayer && WorldGen.genRand.NextBool(300))
                    {
                        if (NobleMushroomsNew.Place(i, j))
                            return;
                    }
                    break;
            }
            if (WorldGen.genRand.NextBool(3000) && j > 200 && !Main.tile[i, j].active() && Framing.GetTileSafely(i, j + 1).active()
                && Main.tileSolid[Main.tile[i, j + 1].type] && Main.tile[i, j].liquid > 0 && !Main.tile[i, j].lava() && !Main.tile[i, j].honey())
            {
                Main.tile[i, j].active(active: true);
                Main.tile[i, j].halfBrick(halfBrick: false);
                Main.tile[i, j].slope(slope: 0);
                Main.tile[i, j].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                Main.tile[i, j].frameX = (short)(22 * ExoticCoralNew.GetRandomStyle(WorldGen.genRand.Next(3)));
                Main.tile[i, j].frameY = 0;
                return;
            }
        }

        public static bool ProtectedTile(int i, int j)
        {
            var tile = Main.tile[i, j];
            if (tile.type > Main.maxTileSets && Main.tileFrameImportant[tile.type])
            {
                bool blockDamaged = false;
                if (TileLoader.GetTile(tile.type).mod.Name == "AQMod")
                {
                    if (!TileLoader.CanKillTile(i, j, tile.type, ref blockDamaged))
                        return true;
                }
            }
            return false;
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            if (j > 1)
            {
                if (ProtectedTile(i, j - 1))
                    return false;
            }
            return true;
        }

        public override bool CanExplode(int i, int j, int type)
        {
            if (j > 1)
            {
                if (ProtectedTile(i, j - 1))
                    return false;
            }
            return true;
        }

        public override bool Slope(int i, int j, int type)
        {
            if (j > 1)
            {
                if (ProtectedTile(i, j - 1))
                    return false;
            }
            return true;
        }

        public static bool CheckForType(Rectangle rect, AQUtils.ArrayInterpreter<int> type)
        {
            return CheckTiles(rect, (i, j, tile) => type.Arr.Contains(tile.type));
        }
        public static bool CheckTiles(Rectangle rect, Func<int, int, Tile, bool> function)
        {
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                        continue;
                    }
                    if (!function(i, j, Main.tile[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static class WindFXHelper
        {
            public static bool WindBlocked(int i, int j)
            {
                return WindBlocked(Main.tile[i, j]);
            }
            public static bool WindBlocked(Tile tile)
            {
                return tile.wall != 0;
            }
        }
    }
}