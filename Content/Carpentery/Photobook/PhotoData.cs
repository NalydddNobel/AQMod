using Aequus.NPCs.CarpenterNPC.Shop;
using Aequus.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Carpentery.Photobook
{
    public struct PhotoData : TagSerializable
    {
        public bool HasData => tileMap != null;

        public float fracX;
        public float fracY;
        public ushort paddingX;
        public ushort paddingY;
        public long date;
        public PhotoGameState gameState;
        public TileMapCache tileMap;

        public Ref<RenderTarget2D> Texture;

        public int Width => tileMap.Width;
        public int Height => tileMap.Height;
        public DateTime Date => DateTime.FromBinary(date);

        public static PhotoData TakeASnap(Rectangle rectangle, ushort paddingX = 6, ushort paddingY = 6)
        {
            var photo = new PhotoData
            {
                fracX = rectangle.X / (float)Main.maxTilesX,
                fracY = rectangle.Y / (float)Main.maxTilesY,
                paddingX = paddingX,
                paddingY = paddingY,
                tileMap = new TileMapCache(rectangle),
                gameState = PhotoGameState.Current(),
                date = DateTime.Now.ToBinary(),
            };
            return photo;
        }

        public void LoadTexture()
        {
            if (Texture == null)
            {
                Texture = new Ref<RenderTarget2D>();
            }
            if (Texture.Value == null)
                PhotoRenderer.RenderRequests.Add(this);
        }

        internal static PhotoData FromLegacyClip(ShutterstockerClip clip)
        {
            var photo = new PhotoData
            {
                fracX = clip.worldXPercent,
                fracY = clip.worldYPercent,
                tileMap = clip.tileMap,
                gameState = new PhotoGameState() { isDayTime = clip.daytime, time = (ushort)clip.time, },
                date = clip.timeCreatedSerialized,
                Texture = clip.TooltipTexture,
                paddingX = 6,
                paddingY = 6,
            };
            return photo;
        }

        public TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["Map"] = tileMap.SerializeData(),
                ["FracX"] = fracX,
                ["FracY"] = fracY,
                ["PaddingX"] = paddingX,
                ["PaddingY"] = paddingY,
                ["GameState"] = gameState.SerializeData(),
                ["Date"] = date,
            };
        }

        public static PhotoData DeserializeData(TagCompound tag)
        {
            var photoData = new PhotoData();
            if (tag.TryGet<TagCompound>("Map", out var map))
            {
                photoData.tileMap = TileMapCache.DeserializeData(map);
                photoData.fracX = tag.Get<float>("FracX");
                photoData.fracY = tag.Get<float>("FracY");
                photoData.paddingX = tag.Get<ushort>("PaddingX");
                photoData.paddingY = tag.Get<ushort>("PaddingY");
                if (tag.TryGet<TagCompound>("GameState", out var gameState))
                {
                    photoData.gameState = PhotoGameState.DeserialzeData(gameState);
                }
                photoData.date = tag.Get<long>("Date");
            }
            return photoData;
        }
    }
}