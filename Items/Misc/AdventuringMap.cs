using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Misc
{
    public class AdventuringMap : ModItem
    {
        public override string Texture => Aequus.VanillaTexture + "Item_" + ItemID.TreasureMap;

        protected override bool CloneNewInstances => true;

        public static int DefaultMapWidth = 40;
        public static int DefaultMapHeight = 30;

        public Point worldCoordinates;
        public int mapWidth;
        public int mapHeight;
        public int worldID;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            worldCoordinates = new Point();
            mapWidth = DefaultMapWidth;
            mapHeight = DefaultMapHeight;
            worldID = -1;
            if (!Main.gameMenu)
            {
                Randomize();
            }
        }

        public void Randomize()
        {
            worldID = Main.worldID;
            worldCoordinates.X = WorldGen.genRand.Next(100, Main.maxTilesX + 50);
            for (int j = 50; j < Main.maxTilesY; j++)
            {
                if (Main.tile[worldCoordinates.X, j].IsFullySolid())
                {
                    worldCoordinates.Y = j;
                    break;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (worldCoordinates == Point.Zero)
            {
                Randomize();
            }

            int index = tooltips.GetIndex("Tooltip#");
            if (worldID != Main.worldID)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "InvalidWorld", AequusText.GetText("ItemTooltip.AdventuringMap.InvalidWorld")) { OverrideColor = new Color(255, 100, 100, 255) });
                return;
            }

            var font = FontAssets.MouseText.Value;
            var measurement = font.MeasureString(AequusHelpers.AirCharacter.ToString());
            string t = "";
            int textW = (int)(mapWidth * 10 * Main.inventoryScale / measurement.X);
            int textH = (int)(mapHeight * 10 * Main.inventoryScale / measurement.Y);
            for (int i = 0; i < textW + 1; i++)
            {
                t += AequusHelpers.AirCharacter;
            }
            for (int i = 0; i < textH; i++)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "Fake" + (textH - i - 1), t));
            }
            tooltips.Insert(index + 1, new TooltipLine(Mod, "Map", t));
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == nameof(Aequus) && line.Name == "Map")
            {

                DrawMap(new Vector2(line.X + mapWidth * 10f * Main.inventoryScale, line.Y + mapHeight * 10f * Main.inventoryScale));
                return false;
            }
            return true;
        }

        private void DrawMap(Vector2 where)
        {
            var rect = GetTileRectangle();
            if (Main.netMode == NetmodeID.MultiplayerClient && Main.GameUpdateCount % 60 == 0)
            {
                int endSectionX = Netplay.GetSectionX(rect.X + rect.Width);
                int endSectionY = Netplay.GetSectionY(rect.Y + rect.Height);
                for (int i = Netplay.GetSectionX(rect.X); i <= endSectionX; i++)
                {
                    for (int j = Netplay.GetSectionY(rect.Y); j < endSectionY; j++)
                    {
                        PacketSystem.Send((w) =>
                        {
                            w.Write(Main.myPlayer);
                            w.Write(i);
                            w.Write(j);
                            return true;
                        }, PacketType.RequestTileSectionFromServer);
                    }
                }
            }

            var pixel = ModContent.Request<Texture2D>(Aequus.AssetsPath + "Pixel").Value;
            int tileSize = (int)(10 * Main.inventoryScale);
            int drawX = (int)(where.X + (mapWidth * -10 + 20) * Main.inventoryScale);
            int drawY = (int)(where.Y + (mapHeight * -10 + 20) * Main.inventoryScale);
            int k = 0;
            int l = 0;
            Main.spriteBatch.Draw(TextureAssets.Map.Value, new Rectangle(drawX - 14, drawY - 4, tileSize * mapWidth + 28, tileSize * mapHeight + 8), Color.White);
            for (int i = rect.X; i < rect.X + rect.Width; i++)
            {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++)
                {
                    MapTile mapTile = Main.Map[i, j];
                    bool grayScale = mapTile.Light < 10;
                    if (mapTile.Light == 0)
                    {
                        mapTile = MapHelper.CreateMapTile(i, j, 0);
                        Main.Map.SetTile(i, j, ref mapTile);
                    }
                    var clr = MapHelper.GetMapTileXnaColor(ref mapTile);
                    if (grayScale)
                    {
                        var hsl = Main.rgbToHsl(clr);
                        hsl.Y = 0f;
                        clr = Main.hslToRgb(hsl);
                    }
                    Main.spriteBatch.Draw(pixel, new Rectangle(drawX + tileSize * l, drawY + tileSize * k, tileSize, tileSize), clr);
                    k++;
                }
                k = 0;
                l++;
            }
            Main.spriteBatch.Draw(TextureAssets.MapDeath.Value, new Vector2(drawX + tileSize * mapWidth / 2f, drawY + tileSize * (mapHeight + 12) / 2f) - new Vector2(8f) * Main.inventoryScale, null, Color.White, 0f, TextureAssets.MapDeath.Value.Size() / 2f, Main.inventoryScale, SpriteEffects.None, 0f);
        }

        private Rectangle GetTileRectangle()
        {
            return new Rectangle(worldCoordinates.X - mapWidth / 2, worldCoordinates.Y - mapHeight / 2 - 6, mapWidth, mapHeight).Fluffize(10);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["WorldCoordsX"] = worldCoordinates.X;
            tag["WorldCoordsY"] = worldCoordinates.Y;
            if (mapWidth != DefaultMapWidth)
                tag["MapWidth"] = mapWidth;
            if (mapHeight != DefaultMapHeight)
                tag["MapHeight"] = mapHeight;
            tag["WorldID"] = worldID;
        }

        public override void LoadData(TagCompound tag)
        {
            worldCoordinates.X = tag.Get<int>("WorldCoordsX");
            worldCoordinates.Y = tag.Get<int>("WorldCoordsY");
            if (tag.TryGet("MapWidth", out int mapWidthSet))
            {
                mapWidth = mapWidthSet;
            }
            if (tag.TryGet("MapHeight", out int mapHeightSet))
            {
                mapHeight = mapHeightSet;
            }
            worldID = tag.Get<int>("WorldID");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(worldCoordinates.X);
            writer.Write(worldCoordinates.Y);
            var bb = new BitsByte(mapWidth != DefaultMapWidth, mapHeight != DefaultMapHeight);
            writer.Write(bb);
            if (bb[0])
                writer.Write(mapWidth);
            if (bb[1])
                writer.Write(mapHeight);
            writer.Write(worldID);
        }

        public override void NetReceive(BinaryReader reader)
        {
            worldCoordinates.X = reader.ReadInt32();
            worldCoordinates.Y = reader.ReadInt32();
            var bb = (BitsByte)reader.ReadByte();
            mapWidth = DefaultMapWidth;
            mapWidth = DefaultMapHeight;
            if (bb[0])
                mapWidth = reader.ReadInt32();
            if (bb[1])
                mapHeight = reader.ReadInt32();
            worldID = reader.ReadInt32();
        }
    }
}