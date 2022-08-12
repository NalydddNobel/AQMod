using Aequus.Common;
using Aequus.Graphics.RenderTargets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Tools.CarpenterTools
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
            //Item.ammo = AmmoID;
            //Item.consumable = true;
        }

        public void SetClip(Rectangle area)
        {
            tileMap = new TileMapCache(area);
            worldXPercent = area.X / (float)Main.maxTilesX;
            worldYPercent = area.Y / (float)Main.maxTilesY;
            daytime = Main.dayTime;
            time = (int)Main.time;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Map"] = tileMap.SerializeData();
            tag["WorldXPercent"] = worldXPercent;
            tag["WorldYPercent"] = worldYPercent;
            tag["Daytime"] = daytime;
            tag["Time"] = time;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<TagCompound>("Map", out var val))
                tileMap = TileMapCache.DeserializeData(val);

            worldXPercent = tag.Get<float>("WorldXPercent");
            worldYPercent = tag.Get<float>("WorldYPercent");
            daytime = tag.Get<bool>("Daytime");
            time = tag.Get<int>("Time");
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
            return clone;
        }

        public override bool CanBeConsumedAsAmmo(Item weapon, Player player)
        {
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!HasTooltipTexture || (Main.mouseRight && Main.mouseRightRelease))
            {
                if (TooltipTexture == null)
                    TooltipTexture = new Ref<RenderTarget2D>();

                ShutterstockerSceneRenderer.renderRequests.Add(this);
                return;
            }

            int index = tooltips.GetIndex("Tooltip#");
            Item.AequusTooltips().FitTooltipBackground(tooltips, TooltipTexture.Value.Width, TooltipTexture.Value.Height, index, "Image");
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Mod == "Aequus" && line.Name == "Image")
            {
                if (HasTooltipTexture)
                {
                    Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + new Vector2(8f), null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (Aequus.HQ)
                    {
                        foreach (var c in AequusHelpers.CircularVector(8))
                        {
                            Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + new Vector2(8f) + c * 4f, null, Color.Black * 0.2f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                        foreach (var c in AequusHelpers.CircularVector(32))
                        {
                            Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y) + c * 2f, null, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        }
                    }
                    Main.spriteBatch.Draw(TooltipTexture.Value, new Vector2(line.X, line.Y), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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