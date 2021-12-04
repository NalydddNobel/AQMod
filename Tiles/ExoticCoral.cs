using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace AQMod.Tiles
{
    public class ExoticCoral : ModTile
    {
        public static int GetStyle(int style, UnifiedRandom random)
        {
            switch (style)
            {
                default:
                return style;
                case 0:
                case 1:
                case 2:
                return style + (random.NextBool() ? 0 : 3);
                case 3:
                case 4:
                case 5:
                return style - (random.NextBool() ? 0 : 3);
            }
        }

        public static bool TryPlaceExoticBlotch(int x, int y, int style, int size)
        {
            var halfSize = size / 2;
            var area = new Rectangle(x - halfSize, y - halfSize, size, size);
            if (area.X < 10)
            {
                area.X = 10;
            }
            else if (area.X + area.Width > Main.maxTilesX - 10)
            {
                area.X = Main.maxTilesX - 10 - area.Width;
            }
            int required = size / 4;
            var validSpots = new List<Point>();
            var tileType = ModContent.TileType<ExoticCoral>();
            for (int j = area.Y; j < area.Y + size; j++)
            {
                for (int i = area.X; i < area.X + size; i++)
                {
                    if (Framing.GetTileSafely(i, j + 1).active() && Main.tileSolid[Main.tile[i, j + 1].type] && (Main.tile[i, j + 1].type == TileID.Stone || Main.tile[i, j + 1].type == TileID.Dirt) &&
                        (!Framing.GetTileSafely(i, j).active() || Main.tileCut[Main.tile[i, j].type]) &&
                        Main.tile[i, j].liquid > 0 && !Main.tile[i, j].lava() && !Main.tile[i, j].honey())
                    {
                        validSpots.Add(new Point(i, j));
                    }
                }
            }
            if (validSpots.Count < required)
                return false;
            for (int i = 0; i < required; i++)
            {
                int index = WorldGen.genRand.Next(validSpots.Count);
                WorldGen.PlaceTile(validSpots[index].X, validSpots[index].Y, (ushort)tileType, true, true, -1, ExoticCoral.GetStyle(style, WorldGen.genRand));
                validSpots.RemoveAt(index);
            }
            return true;
        }

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.CoordinateHeights = new[] { 20, };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 6;
            TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
            TileObjectData.newSubTile.RandomStyleRange = 3;
            TileObjectData.addSubTile(8);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(245, 122, 122), CreateMapEntryName("ExoticCoral"));
            dustType = DustID.Dirt;
            disableSmartCursor = true;
        }

        private const int GlowDistance = 600;

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float intensityMult = 0.1f;
            var screenCenter = AQMod.ScreenCenter;
            var screenPosition = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
            var distance = (screenCenter - screenPosition).Length();
            if (distance < GlowDistance)
            {
                intensityMult += 1f - (distance) / GlowDistance;
            }
            float time = Main.GlobalTime * 2f + (i + j) * 0.125f;
            switch (Main.tile[i, j].frameX / 18)
            {
                case 0:
                case 3:
                {
                    r = ((float)Math.Sin(time) + 1f) / 4f;
                    g = ((float)Math.Cos(time) + 1f) / 16f;
                    b = ((float)Math.Sin(time * 0.85f) + 1f) / 16f;
                }
                break;

                case 1:
                case 4:
                {
                    r = ((float)Math.Sin(time) + 1f) / 16f;
                    g = ((float)Math.Cos(time) + 1f) / 4f;
                    b = ((float)Math.Sin(time * 0.85f) + 1f) / 16f;
                }
                break;

                case 2:
                case 5:
                {
                    r = ((float)Math.Sin(time) + 1f) / 16f;
                    g = ((float)Math.Cos(time) + 1f) / 16f;
                    b = ((float)Math.Sin(time * 0.85f) + 1f) / 4f;
                }
                break;

                case 8:
                {
                    r = ((float)Math.Sin(time * 4f) + 1f) / 4f;
                    g = ((float)Math.Cos(time * 4f) + 1f) / 16f;
                    b = ((float)Math.Sin(time * 3.85f) + 1f) / 16f;
                }
                break;

                case 9:
                {
                    r = ((float)Math.Sin(time * 4f) + 1f) / 16f;
                    g = ((float)Math.Cos(time * 4f) + 1f) / 4f;
                    b = ((float)Math.Sin(time * 3.85f) + 1f) / 16f;
                }
                break;

                case 10:
                {
                    r = ((float)Math.Sin(time * 4f) + 1f) / 16f;
                    g = ((float)Math.Cos(time * 4f) + 1f) / 16f;
                    b = ((float)Math.Sin(time * 3.85f) + 1f) / 4f;
                }
                break;
            }
            r *= intensityMult;
            g *= intensityMult;
            b *= intensityMult;
        }

        public override bool CreateDust(int i, int j, ref int type)
        {
            switch (Main.tile[i, j].frameX / 18)
            {
                case 0:
                case 3:
                case 8:
                {
                    type = ModContent.DustType<ArgonDust>();
                }
                break;

                case 1:
                case 4:
                case 9:
                {
                    type = ModContent.DustType<KryptonDust>();
                }
                break;

                case 2:
                case 5:
                case 10:
                {
                    type = ModContent.DustType<XenonDust>();
                }
                break;
            }
            return true;
        }

        public override bool Drop(int i, int j)
        {
            switch (Main.tile[i, j].frameX / 18)
            {
                default:
                Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.ExoticCoral>());
                break;

                case 8:
                case 9:
                case 10:
                Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.ExoticStarfish>());
                break;
            }
            return true;
        }
    }
}