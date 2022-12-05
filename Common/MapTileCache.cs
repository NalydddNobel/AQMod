using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Map;

namespace Aequus.Common
{
    public struct MapTileCache
    {
        public int width;
        public int height;
        public Color[] colorLookup;
        public MapTile[] mapTiles;

        public void UpdateColorLookup()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int index = i + j * width;
                    colorLookup[index] = MapHelper.GetMapTileXnaColor(ref mapTiles[index]);
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
                writer.Write(mapTiles[i].Type);
                writer.Write(mapTiles[i].Color);
            }
        }

        public static MapTileCache NetReceive(BinaryReader reader)
        {
            var map = new MapTileCache
            {
                width = reader.ReadInt32(),
                height = reader.ReadInt32()
            };
            map.mapTiles = new MapTile[map.width * map.height];
            map.colorLookup = new Color[map.width * map.height];
            for (int i = 0; i < map.mapTiles.Length; i++)
            {
                map.mapTiles[i] = MapTile.Create(reader.ReadUInt16(), byte.MaxValue, reader.ReadByte());
            }
            return map;
        }

        public override string ToString()
        {
            return $"{width}x{height}, {(mapTiles == null ? "null" : $"mapTiles length:{mapTiles.Length}")}, {(colorLookup == null ? "null" : $"color lookup length:{colorLookup.Length}")}";
        }
    }
}