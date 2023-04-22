using Aequus.Content.Town.CarpenterNPC.Misc;
using Aequus.Content.Town.CarpenterNPC.Quest;
using Aequus.Tiles.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Town.CarpenterNPC.Rewards {
    public class PixelCameraClip : PhotoClipBase<RenderTarget2D>
    {
        public PixelPaintingData mapCache;
        public int photoState;

        public int Width { get => mapCache.width; set => mapCache.width = value; }
        public int Height { get => mapCache.height; set => mapCache.height = value; }
        public Color[] ColorLookup { get => mapCache.colorLookup; set => mapCache.colorLookup = value; }

        public override float BaseTooltipTextureScale => 4f;
        public override int OldItemID => ModContent.ItemType<PixelCameraClipAmmo>();

        public override ModItem Clone(Item newEntity)
        {
            var clone = (PixelCameraClip)base.Clone(newEntity);
            clone.mapCache = mapCache;
            clone.photoState = photoState;
            return clone;
        }

        public override void SetStaticDefaults() {
            base.SetStaticDefaults();
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            UpdateState();
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
            var MapTiles = new MapTile[area.Width * area.Height];
            ColorLookup = new Color[area.Width * area.Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    int index = i + j * Width;
                    MapTiles[index] = MapHelper.CreateMapTile(area.X + i, area.Y + j, byte.MaxValue);
                }
            }
            mapCache.InheritMapTiles(MapTiles);
        }

        public override void OnMissingTooltipTexture()
        {
            if (TooltipTexture == null)
            {
            }
            PixelCameraRenderer.RenderRequests.Add(new PixelCameraRenderer.RequestInfo() { width = Width, height = Height, arr = ColorLookup, target = TooltipTexture, });
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            mapCache.Save(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            mapCache = PixelPaintingData.Load(tag);
            photoState = tag.Get<int>("PhotoState");
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
            mapCache = PixelPaintingData.NetReceive(reader);
        }
    }

    public class PixelCameraClipAmmo : ModItem
    {
        public static int AmmoID => ModContent.ItemType<PixelCameraClipAmmo>();

        public override string Texture => ModContent.GetInstance<PixelCameraClip>().Texture;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            // DisplayName.SetDefault("{$Mods.Aequus.ItemName.PixelCameraClip}");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = Item.CommonMaxStack;
            Item.ammo = AmmoID;
            Item.consumable = true;
        }
    }
}