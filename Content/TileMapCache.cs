using Microsoft.Xna.Framework;
using MonoMod.Utils;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content
{
    public class TileMapCache : TagSerializable
    {
        public readonly Rectangle OriginalAreaInWorld;
        public readonly int WorldID;

        private TileDataCache[,] cachedInfo;

        internal TileMapCache(Rectangle area, TileDataCache[,] cachedInfo, int worldID)
        {
            OriginalAreaInWorld = area;
            WorldID = worldID;
            this.cachedInfo = cachedInfo;
        }

        public TileMapCache(Rectangle worldFrame)
        {
            OriginalAreaInWorld = worldFrame;
            WorldID = Main.worldID;
            cachedInfo = new TileDataCache[worldFrame.Width, worldFrame.Height];
            Setup();
        }

        public TileDataCache Get(int i, int j)
        {
            return cachedInfo[i, j];
        }

        public TileDataCache Get(Point point)
        {
            return cachedInfo[point.X, point.Y];
        }

        public TileDataCache this[int i, int j]
        {
            get
            {
                return cachedInfo[i, j];
            }
        }

        public TileDataCache this[Point point]
        {
            get
            {
                return cachedInfo[point.X, point.Y];
            }
        }

        public bool InMap(int x, int y)
        {
            return x > -1 && x < OriginalAreaInWorld.Width && y > -1 && y < OriginalAreaInWorld.Height;
        }

        public void Setup()
        {
            for (int i = 0; i < OriginalAreaInWorld.Width; i++)
            {
                for (int j = 0; j < OriginalAreaInWorld.Height; j++)
                {
                    cachedInfo[i, j] = new TileDataCache(Main.tile[OriginalAreaInWorld.X + i, OriginalAreaInWorld.Y + j]);
                }
            }
        }

        public virtual TagCompound SerializeData()
        {
            var tag = new TagCompound()
            {
                ["Area"] = CompressArea(),
                ["Tiles"] = CompressTileArray(),
                ["WorldID"] = WorldID,
            };
            return tag;
        }

        public static TileMapCache DeserializeData(TagCompound tag)
        {
            int[] rectCompressed = tag.GetIntArray("Area");
            byte[] tilesCompressed = tag.GetByteArray("Tiles");
            int worldID = tag.GetInt("WorldID");
            return new TileMapCache(new Rectangle(rectCompressed[0], rectCompressed[1], rectCompressed[2], rectCompressed[3]),
                DecompressInfo(rectCompressed[2], rectCompressed[3], tilesCompressed), worldID);
        }

        public int[] CompressArea()
        {
            return new int[] { OriginalAreaInWorld.X, OriginalAreaInWorld.Y, OriginalAreaInWorld.Width, OriginalAreaInWorld.Height, };
        }

        public byte[] CompressTileArray()
        {
            int bytesUsedByTiles = OriginalAreaInWorld.Width * OriginalAreaInWorld.Height * 12;
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write(Main.maxTileSets);
                var conversionTable = new Dictionary<int, string>();
                for (int i = 0; i < OriginalAreaInWorld.Width; i++)
                {
                    for (int j = 0; j < OriginalAreaInWorld.Height; j++)
                    {
                        if (cachedInfo[i, j].Type.Type >= Main.maxTileSets && !conversionTable.ContainsKey(cachedInfo[i, j].Type.Type))
                        {
                            conversionTable.Add(cachedInfo[i, j].Type.Type, TileLoader.GetTile(cachedInfo[i, j].Type.Type).FullName);
                        }
                    }
                }

                writer.Write(conversionTable.Count);
                foreach (var pair in conversionTable)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
                long position = writer.BaseStream.Position;
                writer.Write((long)0);

                for (int i = 0; i < OriginalAreaInWorld.Width; i++)
                {
                    for (int j = 0; j < OriginalAreaInWorld.Height; j++)
                    {
                        writer.Write(cachedInfo[i, j].Type.Type);
                        writer.Write(cachedInfo[i, j].Liquid.Amount);
                        writer.Write(TileDataCache.TileReflectionHelper.LiquidData_typeAndFlags.GetValue<byte>(cachedInfo[i, j].Liquid));
                        writer.Write(cachedInfo[i, j].Misc.TileFrameX);
                        writer.Write(cachedInfo[i, j].Misc.TileFrameY);
                        writer.Write(TileDataCache.TileReflectionHelper.TileWallWireStateData_bitpack.GetValue<int>(cachedInfo[i, j].Misc));
                    }
                }

                long length = writer.BaseStream.Position;
                writer.BaseStream.Position = position;
                writer.Write(length);
                return stream.GetBuffer();
            }
        }

        public static TileDataCache[,] DecompressInfo(int width, int height, byte[] buffer)
        {
            using (var reader = new BinaryReader(new MemoryStream(buffer)))
            {
                var info = new TileDataCache[width, height];
                int maxTiles = reader.ReadInt32();
                int conversionCount = reader.ReadInt32();
                var conversionTable = new Dictionary<int, int>();
                
                for (int i = 0; i < conversionCount; i++)
                {
                    int tileResultID = -1;
                    int tileConversionKey = reader.ReadInt32();
                    var tileKey = reader.ReadString();
                    var split = tileKey.Split('/');
                    if (ModLoader.TryGetMod(split[0], out var mod))
                    {
                        if (mod.TryFind<ModTile>(split[1], out var modTile))
                        {
                            tileResultID = modTile.Type;
                            if (tileResultID == tileConversionKey)
                                continue;
                        }
                    }
                    conversionTable.Add(tileConversionKey, tileResultID);
                }

                long uselessMaxBytes = reader.ReadInt64();

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var tileType = new TileTypeData()
                        {
                            Type = reader.ReadUInt16(),
                        };
                        if (tileType.Type >= maxTiles && conversionTable.TryGetValue(tileType.Type, out int val))
                        {
                            tileType.Type = (ushort)val;
                        }
                        var liquid = new LiquidData
                        {
                            Amount = reader.ReadByte(),
                        };
                        object box = liquid;
                        TileDataCache.TileReflectionHelper.LiquidData_typeAndFlags.SetValue(box, reader.ReadByte());
                        liquid = (LiquidData)box;
                        var misc = new TileWallWireStateData()
                        {
                            TileFrameX = reader.ReadInt16(),
                            TileFrameY = reader.ReadInt16(),
                        };
                        int val2 = reader.ReadInt32();
                        box = misc;
                        TileDataCache.TileReflectionHelper.TileWallWireStateData_bitpack.SetValue(box, val2);
                        misc = (TileWallWireStateData)box;
                        info[i, j] = new TileDataCache(tileType, liquid, misc);
                    }
                }
                return info;
            }
        }
    }
}