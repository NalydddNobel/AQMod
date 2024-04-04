﻿using Aequus.Items.Tools;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Common;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Tile;
using System.IO;
using Terraria.Map;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Clip;

public class PixelCameraClip : PhotoClipBase<RenderTarget2D> {
    public PixelPaintingData mapCache;
    public int photoState;

    public int Width { get => mapCache.width; set => mapCache.width = value; }
    public int Height { get => mapCache.height; set => mapCache.height = value; }
    public Color[] ColorLookup { get => mapCache.colorLookup; set => mapCache.colorLookup = value; }

    public override float BaseTooltipTextureScale => 4f;
    public override int OldItemID => ModContent.ItemType<PixelCameraClipAmmo>();

    public override ModItem Clone(Item newEntity) {
        var clone = (PixelCameraClip)base.Clone(newEntity);
        clone.mapCache = mapCache;
        clone.photoState = photoState;
        return clone;
    }

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemSets.DisableAutomaticPlaceableDrop[Type] = true;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Item.useTime = 10;
        Item.useAnimation = 15;
        Item.consumable = true;
        Item.useStyle = ItemUseStyleID.Swing;
        UpdateState();
    }

    public override void UpdateInventory(Player player) {
        UpdateState();
    }

    public void UpdateState() {
        if (PixelPaintingTile.PhotoStateToTileID.IndexInRange(photoState)) {
            Item.createTile = PixelPaintingTile.PhotoStateToTileID[photoState];
        }
    }

    public override void SetClip(Rectangle area) {
        base.SetClip(area);
        Width = area.Width;
        Height = area.Height;
        var MapTiles = new MapTile[area.Width * area.Height];
        ColorLookup = new Color[area.Width * area.Height];
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                int index = i + j * Width;
                MapTiles[index] = MapHelper.CreateMapTile(area.X + i, area.Y + j, byte.MaxValue);
            }
        }
        mapCache.InheritMapTiles(MapTiles);
    }

    public override void OnMissingTooltipTexture() {
        if (TooltipTexture == null) {
            PixelCameraRenderer.RenderRequests.Add(new PixelCameraRenderer.RequestInfo() { width = Width, height = Height, arr = ColorLookup, target = TooltipTexture, });
        }
    }

    public override void SaveData(TagCompound tag) {
        base.SaveData(tag);
        mapCache.Save(tag);
    }

    public override void LoadData(TagCompound tag) {
        base.LoadData(tag);
        mapCache = PixelPaintingData.Load(tag);
        photoState = tag.Get<int>("PhotoState");
    }

    public override void NetSend(BinaryWriter writer) {
        base.NetSend(writer);
        writer.Write(photoState);
        mapCache.NetSend(writer);
    }

    public override void NetReceive(BinaryReader reader) {
        base.NetReceive(reader);
        photoState = reader.ReadInt32();
        mapCache = PixelPaintingData.NetReceive(reader);
    }
}