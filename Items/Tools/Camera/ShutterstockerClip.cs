using Aequus.Common;
using Aequus.Graphics.RenderTargets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace Aequus.Items.Tools.Camera
{
    public class ShutterstockerClip : ModItem
    {
        private static TileMapCache DefaultTileMap;

        public float worldXPercent;
        public float worldYPercent;
        public int time;
        public bool daytime;
        public TileMapCache tileMap;
        public Ref<RenderTarget2D> TooltipTexture;
        public bool reviewed;
        public long timeCreatedSerialized;

        public float TooltipTextureScale
        {
            get
            {
                float scale = 1f;
                int maxSize = (Main.screenWidth < Main.screenHeight ? Main.screenWidth : Main.screenHeight) / 2;
                int largestSide = (int)((TooltipTexture.Value.Width > TooltipTexture.Value.Height ? TooltipTexture.Value.Width : TooltipTexture.Value.Height) * scale);

                if (largestSide > maxSize)
                {
                    return maxSize / (float)largestSide * scale;
                }
                return scale;
            }
        }

        public DateTime TimeCreated { get => DateTime.FromBinary(timeCreatedSerialized); set => timeCreatedSerialized = value.ToBinary(); }
        public string TimeCreatedString => TimeCreated.ToString("MM/dd/yy h:mm tt", Language.ActiveCulture.CultureInfo);
        public bool AppendTimeCreatedTextToImage
        {
            get
            {
                var measurement = FontAssets.MouseText.Value.MeasureString(TimeCreatedString);
                return TooltipTexture != null && TooltipTexture.Value != null && TooltipTexture.Value.Height > measurement.Y * 2f && TooltipTexture.Value.Width > measurement.X * 1.1f;
            }
        }

        public bool HasTooltipTexture => TooltipTexture != null && TooltipTexture.Value != null && !TooltipTexture.Value.IsDisposed && !TooltipTexture.Value.IsContentLost;

        public override void SetStaticDefaults()
        {
            InitDefaultMap();
        }

        private void InitDefaultMap()
        {
            var mapData = new TileDataCache[36 + ShutterstockerSceneRenderer.TilePadding, 36 + ShutterstockerSceneRenderer.TilePadding];
            var rect = new Rectangle(0, 0, 36 + ShutterstockerSceneRenderer.TilePadding, 36 + ShutterstockerSceneRenderer.TilePadding);
            var t = new TileDataCache(new TileTypeData() { Type = TileID.Grass, }, new LiquidData(), new TileWallWireStateData() { HasTile = true, }, new WallTypeData());
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    mapData[i, j] = t;
                }
            }

            DefaultTileMap = new TileMapCache(rect, mapData, Main.worldID);
        }

        public override void Unload()
        {
            DefaultTileMap = null;
        }

        public override void SetDefaults()
        {
            if (!Main.gameMenu && Main.netMode != NetmodeID.Server)
            {
                SetClip(Utils.CenteredRectangle(Main.LocalPlayer.Center.ToTileCoordinates().ToVector2(), new Vector2(36f + ShutterstockerSceneRenderer.TilePadding, 36f + ShutterstockerSceneRenderer.TilePadding)));
            }
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

        public void SetClip(Rectangle area)
        {
            tileMap = new TileMapCache(area);
            worldXPercent = area.X / (float)Main.maxTilesX;
            worldYPercent = area.Y / (float)Main.maxTilesY;
            daytime = Main.dayTime;
            time = (int)Main.time;
            reviewed = false;
            TimeCreated = DateTime.Now;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Map"] = tileMap.SerializeData();
            tag["WorldXPercent"] = worldXPercent;
            tag["WorldYPercent"] = worldYPercent;
            tag["Daytime"] = daytime;
            tag["Time"] = time;
            tag["Reviewed"] = reviewed;
            tag["TimeCreated"] = timeCreatedSerialized;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<TagCompound>("Map", out var val))
                tileMap = TileMapCache.DeserializeData(val);

            worldXPercent = tag.Get<float>("WorldXPercent");
            worldYPercent = tag.Get<float>("WorldYPercent");
            daytime = tag.Get<bool>("Daytime");
            time = tag.Get<int>("Time");
            reviewed = tag.Get<bool>("Reviewed");
            timeCreatedSerialized = tag.Get<long>("TimeCreated");
        }

        public override ModItem Clone(Item newEntity)
        {
            var clone = (ShutterstockerClip)base.Clone(newEntity);

            if (tileMap == null)
                tileMap = DefaultTileMap.Clone();
            if (TooltipTexture == null)
                TooltipTexture = new Ref<RenderTarget2D>();

            clone.tileMap = tileMap;
            clone.TooltipTexture = TooltipTexture;
            clone.daytime = daytime;
            clone.time = time;
            clone.reviewed = reviewed;
            clone.timeCreatedSerialized = timeCreatedSerialized;
            return clone;
        }

        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!HasTooltipTexture || Main.mouseRight && Main.mouseRightRelease)
            {
                if (TooltipTexture == null)
                    TooltipTexture = new Ref<RenderTarget2D>();

                ShutterstockerSceneRenderer.renderRequests.Add(this);
                return;
            }

            int index = tooltips.GetIndex("Tooltip#");
            float scale = TooltipTextureScale;
            Item.AequusTooltips().FitTooltipBackground(tooltips, (int)(TooltipTexture.Value.Width * scale), (int)(TooltipTexture.Value.Height * scale), index, "Image");
            bool timeTooltip = !AppendTimeCreatedTextToImage;
            bool updateIndex = timeTooltip || reviewed;

            if (!updateIndex)
            {
                return;
            }

            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Name.StartsWith("Fake"))
                    index = i;
            }

            if (timeTooltip)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "Date", TimeCreatedString) { OverrideColor = Color.Lerp(Color.Yellow, Color.White, 0.5f), });
            }

            if (reviewed)
            {
                tooltips.Insert(index + 1, new TooltipLine(Mod, "Reviewed", AequusText.GetText($"ItemTooltip.{Name}.Reviewed")) { OverrideColor = Color.Lerp(Color.BlueViolet, Color.White, 0.5f), });
            }
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus" && line.Name == "Image")
            {
                if (HasTooltipTexture)
                {
                    //line.Rotation = Main.GlobalTimeWrappedHourly;
                    var scale = line.BaseScale * TooltipTextureScale;
                    Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + new Vector2(8f), null, Color.Black, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    if (Aequus.HQ)
                    {
                        foreach (var c in AequusHelpers.CircularVector(8))
                        {
                            Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + new Vector2(8f) + c * 4f, null, Color.Black * 0.2f, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        }
                        foreach (var c in AequusHelpers.CircularVector(32))
                        {
                            Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + c * 2f, null, Color.Black, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                        }
                    }
                    Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y), null, Color.White, line.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f);
                    if (AppendTimeCreatedTextToImage)
                    {
                        var text = TimeCreatedString;
                        var measurement = FontAssets.MouseText.Value.MeasureString(text);
                        var drawCoords = new Vector2(line.X + 4, line.Y + TooltipTexture.Value.Height * scale.Y - measurement.Y / 2f - 8f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, drawCoords, Color.Lerp(Color.Yellow, Color.White, 0.5f), line.Rotation, Vector2.Zero, line.BaseScale);
                    }
                }
                return false;
            }
            return true;
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (tileMap == null)
            {
                worldXPercent = 0.5f;
                worldYPercent = 0.5f;
                tileMap = DefaultTileMap;
            }

            writer.Write(daytime);
            writer.Write(time);
            writer.Write(worldXPercent);
            writer.Write(worldYPercent);
            writer.Write(tileMap.WorldID);
            writer.Write(tileMap.Area.X);
            writer.Write(tileMap.Area.Y);
            writer.Write(tileMap.Area.Width);
            writer.Write(tileMap.Area.Height);
            var buffer = tileMap.CompressTileArray();
            writer.Write(buffer.Length);
            //Aequus.Instance.Logger.Debug(buffer.Length);
            writer.Write(buffer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            daytime = reader.ReadBoolean();
            time = reader.ReadInt32();
            worldXPercent = reader.ReadSingle();
            worldYPercent = reader.ReadSingle();
            int worldID = reader.ReadInt32();
            var rect = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            var bufferLength = reader.ReadInt32();
            tileMap = new TileMapCache(rect, TileMapCache.DecompressInfo(rect.Width, rect.Height, reader.ReadBytes(bufferLength)), worldID);
        }
    }
}