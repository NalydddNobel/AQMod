using Aequus.Common.Tiles;
using Aequus.NPCs.Town.CarpenterNPC.Quest;
using Aequus.Unused.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs.Town.CarpenterNPC {
    public struct PhotoData : TagSerializable {
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

        public static PhotoData TakeASnap(Rectangle rectangle, ushort paddingX = 6, ushort paddingY = 6) {
            var photo = new PhotoData {
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

        public void LoadTexture() {
            if (Texture == null) {
                Texture = new Ref<RenderTarget2D>();
            }
            if (Texture.Value == null)
                PhotoRenderer.RenderRequests.Add(this);
        }

        public TagCompound SerializeData() {
            return new TagCompound {
                ["Map"] = tileMap.SerializeData(),
                ["FracX"] = fracX,
                ["FracY"] = fracY,
                ["PaddingX"] = paddingX,
                ["PaddingY"] = paddingY,
                ["GameState"] = gameState.SerializeData(),
                ["Date"] = date,
            };
        }

        public static PhotoData DeserializeData(TagCompound tag) {
            var photoData = new PhotoData();
            if (tag.TryGet<TagCompound>("Map", out var map)) {
                photoData.tileMap = TileMapCache.DeserializeData(map);
                photoData.fracX = tag.Get<float>("FracX");
                photoData.fracY = tag.Get<float>("FracY");
                photoData.paddingX = tag.Get<ushort>("PaddingX");
                photoData.paddingY = tag.Get<ushort>("PaddingY");
                if (tag.TryGet<TagCompound>("GameState", out var gameState)) {
                    photoData.gameState = PhotoGameState.DeserialzeData(gameState);
                }
                photoData.date = tag.Get<long>("Date");
            }
            return photoData;
        }
    }
}