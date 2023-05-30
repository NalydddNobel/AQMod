using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Town.CarpenterNPC.Quest {
    public struct PixelPaintingData
    {
        public int width;
        public int height;
        public Color[] colorLookup;
        public Color this[int index]
        {
            get => colorLookup[index];
            set => colorLookup[index] = value;
        }
        public Color this[int x, int y]
        {
            get => colorLookup[x + y * width];
            set => colorLookup[x + y * width] = new Color(value.R, value.G, value.B, 255);
        }
        public int Length => colorLookup.Length;

        public PixelPaintingData(int w, int h)
        {
            width = w;
            height = h;
            colorLookup = new Color[w * h];
            for (int i = 0; i < colorLookup.Length; i++)
            {
                colorLookup[i].A = 255;
            }
        }

        public void InheritMapTiles(MapTile[] arr)
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;
                    colorLookup[index] = MapHelper.GetMapTileXnaColor(ref arr[index]);
                }
            }
        }

        public void NetSend(BinaryWriter writer)
        {
            writer.Write(width);
            writer.Write(height);
            int amt = width * height;
            for (int i = 0; i < amt; i++)
            {
                writer.Write(colorLookup[i].R);
                writer.Write(colorLookup[i].G);
                writer.Write(colorLookup[i].B);
            }
        }

        public static PixelPaintingData NetReceive(BinaryReader reader)
        {
            var map = new PixelPaintingData(reader.ReadInt32(), reader.ReadInt32());
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
            }
            return map;
        }

        private void LoadLegacy(TagCompound tag)
        {
            var mapTileIDs = tag.Get<ushort[]>("MapTileIDs");
            var mapTilePaints = tag.Get<byte[]>("MapTileColor");

            var mapTiles = new MapTile[width * height];
            if (mapTileIDs.Length != mapTilePaints.Length || mapTileIDs.Length != colorLookup.Length || mapTilePaints.Length != colorLookup.Length)
                return;

            for (int i = 0; i < mapTiles.Length; i++)
            {
                mapTiles[i] = MapTile.Create(mapTileIDs[i], byte.MaxValue, mapTilePaints[i]);
            }
            InheritMapTiles(mapTiles);
        }

        public void Save(TagCompound tag)
        {
            if (colorLookup == null || colorLookup.Length != (width * height))
                return;
            tag["Width"] = width;
            tag["Height"] = height;
            tag["ColorsList"] = colorLookup.ToList();
        }

        public static PixelPaintingData Load(TagCompound tag)
        {
            var mapCache = new PixelPaintingData(tag.Get<int>("Width"), tag.Get<int>("Height"));
            if (tag.ContainsKey("MapTileIDs"))
            {
                mapCache.LoadLegacy(tag);
                return mapCache;
            }

            if (tag.TryGet<List<Color>>("ColorsList", out var colorList) && colorList.Count == mapCache.width * mapCache.height)
            {
                for (int i = 0; i < mapCache.Length; i++)
                {
                    mapCache[i] = colorList[i];
                }
            }
            return mapCache;
        }

        public static PixelPaintingData FromTileSection(Rectangle section)
        {
            var data = new PixelPaintingData(section.Width, section.Height);
            var mapTiles = new MapTile[section.Width * section.Height];
            for (int i = 0; i < section.Width; i++)
            {
                for (int j = 0; j < section.Height; j++)
                {
                    int index = i + j * section.Width;
                    mapTiles[index] = MapHelper.CreateMapTile(section.X + i, section.Y + j, byte.MaxValue);
                }
            }
            data.InheritMapTiles(mapTiles);
            return data;
        }

        public override string ToString()
        {
            return $"{width}x{height}, {(colorLookup == null ? "null" : $"color lookup length:{colorLookup.Length}")}";
        }
    }
}