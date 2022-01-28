using AQMod.Common.Graphics;
using AQMod.Dusts.NobleMushrooms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AQMod.Tiles.Nature.CrabCrevice
{
    public class ExoticCoralNew : ModTile
    {
        private const int MinimumDistanceForExoticCoralToGlow = 600;

        public static int[] AnchorValidTiles => TileObjectData.GetTileData(ModContent.TileType<ExoticCoralNew>(), 0, 0).AnchorValidTiles;

        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.DrawYOffset += -10;
            TileObjectData.newTile.CoordinateWidth = 20;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 28, };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 4;
            TileObjectData.newTile.StyleMultiplier = 4;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.OnlyInLiquid;
            TileObjectData.newTile.AnchorValidTiles = new int[]
            {
                TileID.Dirt,
                TileID.Stone,
                TileID.Obsidian,
                TileID.Sand,
                TileID.HardenedSand,
                ModContent.TileType<SedimentSand>(),
            };
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(255, 0, 148), CreateMapEntryName("ExoticCoral"));
            AddMapEntry(new Color(0, 232, 95), CreateMapEntryName("ExoticCoral"));
            AddMapEntry(new Color(2, 254, 242), CreateMapEntryName("ExoticCoral"));
            dustType = DustID.Dirt;
            disableSmartCursor = true;
        }

        public override ushort GetMapOption(int i, int j)
        {
            ushort option = (ushort)(Main.tile[i, j].frameX / 88);
            return option;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float intensityMult = 0.1f;
            var screenCenter = AQGraphics.ScreenCenter;
            var screenPosition = new Vector2(i * 16f, j * 16f) - Main.screenPosition;
            var distance = (screenCenter - screenPosition).Length();
            if (distance < MinimumDistanceForExoticCoralToGlow)
                intensityMult += 1f - distance / MinimumDistanceForExoticCoralToGlow;
            float time = AQGraphics.TimerBasedOnTimeOfDay * 2f + (i + j) * 0.125f;
            switch (Main.tile[i, j].frameX / 22)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    {
                        r = ((float)Math.Sin(time) + 1f) / 4f;
                        g = ((float)Math.Cos(time) + 1f) / 16f;
                        b = ((float)Math.Sin(time * 0.85f) + 1f) / 16f;
                    }
                    break;

                case 4:
                case 5:
                case 6:
                case 7:
                    {
                        r = ((float)Math.Sin(time) + 1f) / 16f;
                        g = ((float)Math.Cos(time) + 1f) / 4f;
                        b = ((float)Math.Sin(time * 0.85f) + 1f) / 16f;
                    }
                    break;

                case 8:
                case 9:
                case 10:
                case 11:
                    {
                        r = ((float)Math.Sin(time) + 1f) / 16f;
                        g = ((float)Math.Cos(time) + 1f) / 16f;
                        b = ((float)Math.Sin(time * 0.85f) + 1f) / 4f;
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
                case 1:
                case 2:
                case 3:
                    {
                        type = ModContent.DustType<ArgonDust>();
                    }
                    break;

                case 4:
                case 5:
                case 6:
                case 7:
                    {
                        type = ModContent.DustType<KryptonDust>();
                    }
                    break;

                case 8:
                case 9:
                case 10:
                case 11:
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
                    Item.NewItem(i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.Nature.ExoticCoral>());
                    break;
            }
            return true;
        }

        public static bool CanBePlacedOnType(int type)
        {
            foreach (int t in AnchorValidTiles)
            {
                if (t == type)
                    return true;
            }
            return false;
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
                Main.tile[validSpots[index].X, validSpots[index].Y].active(active: true);
                Main.tile[validSpots[index].X, validSpots[index].Y].halfBrick(halfBrick: false);
                Main.tile[validSpots[index].X, validSpots[index].Y].slope(slope: 0);
                Main.tile[validSpots[index].X, validSpots[index].Y].type = (ushort)ModContent.TileType<ExoticCoralNew>();
                Main.tile[validSpots[index].X, validSpots[index].Y].frameX = (short)(22 * GetRandomStyle(style));
                Main.tile[validSpots[index].X, validSpots[index].Y].frameY = 0;
                validSpots.RemoveAt(index);
            }
            return true;
        }

        public static int GetRandomStyle(int style)
        {
            style = style * 4 + WorldGen.genRand.Next(4);
            return style;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var drawFrame = new Rectangle(Main.tile[i, j].frameX, 0, 20, 28);
            var drawOrigin = new Vector2(10f, 18f);
            var drawPosition = new Vector2(i * 16f + drawOrigin.X, j * 16f + drawOrigin.Y - 10f);
            float rotation = 0f;
            if (j < (int)Main.worldSurface && Main.tile[i, j].wall == 0)
            {
                float windPower = ((float)Math.Cos(Main.GlobalTime * MathHelper.Pi + i * 0.1f) + 1f) / 2f * Main.windSpeed;
                drawPosition.X += windPower;
                drawPosition.Y += Math.Abs(windPower);
                rotation = windPower * 0.1f;
            }

            Main.spriteBatch.Draw(Main.tileTexture[Type], drawPosition - Main.screenPosition + AQGraphics.TileZero, drawFrame,
                Lighting.GetColor(i, j), rotation, drawOrigin, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}