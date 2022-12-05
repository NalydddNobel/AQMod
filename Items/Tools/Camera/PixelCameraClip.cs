using Aequus.Common;
using Aequus.Graphics.RenderTargets;
using Aequus.Tiles.Furniture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Items.Tools.Camera
{
    public class PixelCameraClip : PhotoClipBase<RenderTarget2D>
    {
        public MapTileCache mapCache;
        public int photoState;

        public int Width { get => mapCache.width; set => mapCache.width = value; }
        public int Height { get => mapCache.height; set => mapCache.height = value; }
        public Color[] ColorLookup { get => mapCache.colorLookup; set => mapCache.colorLookup = value; }
        public MapTile[] MapTiles { get => mapCache.mapTiles; set => mapCache.mapTiles = value; }

        public override float BaseTooltipTextureScale => 4f;

        public override ModItem Clone(Item newEntity)
        {
            var clone = (PixelCameraClip)base.Clone(newEntity);
            clone.mapCache = mapCache;
            clone.photoState = photoState;
            mapCache.UpdateColorLookup();
            return clone;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            UpdateState();
            mapCache.mapTiles = null;
            mapCache.colorLookup = null;
        }

        public override void UpdateInventory(Player player)
        {
            UpdateState();
        }

        public void UpdateState()
        {
            if (photoState >= 0 && photoState < PixelPaintingTile.PhotoStateToTileID.Length)
            {
                Item.createTile = PixelPaintingTile.PhotoStateToTileID[photoState];
            }
        }

        public override void SetClip(Rectangle area)
        {
            base.SetClip(area);
            Width = area.Width;
            Height = area.Height;
            MapTiles = new MapTile[area.Width * area.Height];
            ColorLookup = new Color[area.Width * area.Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    int index = i + j * Width;
                    MapTiles[index] = MapHelper.CreateMapTile(area.X + i, area.Y + j, byte.MaxValue);
                }
            }
            mapCache.UpdateColorLookup();
        }

        public override void OnMissingTooltipTexture()
        {
            if (TooltipTexture == null)
            {
                mapCache.UpdateColorLookup();
            }
            ColorImageRenderer.RenderRequests.Add(new ColorImageRenderer.RequestInfo() { width = Width, height = Height, arr = ColorLookup, target = TooltipTexture, });
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (MapTiles == null)
                return;

            tag["Width"] = Width;
            tag["Height"] = Height;
            tag["PhotoState"] = photoState;
            tag["MapTileIDs"] = Array.ConvertAll(MapTiles, (t) => t.Type);
            tag["MapTileColor"] = Array.ConvertAll(MapTiles, (t) => t.Color);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);

            Width = tag.Get<int>("Width");
            Height = tag.Get<int>("Height");
            photoState = tag.Get<int>("PhotoState");
            var mapTileIDs = tag.Get<ushort[]>("MapTileIDs");
            var mapTilePaints = tag.Get<byte[]>("MapTileColor");

            ColorLookup = new Color[Width * Height];
            MapTiles = new MapTile[Width * Height];
            mapCache.UpdateColorLookup();
            if (mapTileIDs.Length != mapTilePaints.Length || mapTileIDs.Length != ColorLookup.Length || mapTilePaints.Length != ColorLookup.Length)
                return;

            for (int i = 0; i < MapTiles.Length; i++)
            {
                MapTiles[i] = MapTile.Create(mapTileIDs[i], byte.MaxValue, mapTilePaints[i]);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(photoState);
            mapCache.NetSend(writer);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            photoState = reader.ReadInt32();
            mapCache = MapTileCache.NetReceive(reader);
            mapCache.UpdateColorLookup();
        }
    }

    public class PixelCameraClipAmmo : ModItem
    {
        public static int AmmoID => ModContent.ItemType<PixelCameraClipAmmo>();

        public override string Texture => ModContent.GetInstance<PixelCameraClip>().Texture;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("{$Mods.Aequus.ItemName.PixelCameraClip}");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = 9999;
            Item.ammo = AmmoID;
            Item.consumable = true;
        }
    }
}