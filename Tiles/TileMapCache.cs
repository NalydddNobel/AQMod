using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Tiles {
    public class TileMapCache : TagSerializable
    {
        public readonly Rectangle Area;
        public readonly int WorldID;

        public TileDataCache[,] cachedInfo;

        public const int SaveType = 0;

        public int Width => Area.Width;
        public int Height => Area.Height;

        public Point WorldPointDebug(int x, int y) => new Point(Area.X + x, Area.Y + y);

        public Point WorldPointDebug(Point p) => WorldPointDebug(p.X, p.Y);

        public TileMapCache(Rectangle area, TileDataCache[,] cachedInfo, int worldID)
        {
            Area = area;
            WorldID = worldID;
            this.cachedInfo = cachedInfo;
        }

        public TileMapCache(Rectangle worldFrame)
        {
            Area = worldFrame;
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
            set
            {
                cachedInfo[i, j] = value;
            }
        }

        public TileDataCache this[Point point]
        {
            get
            {
                return cachedInfo[point.X, point.Y];
            }
            set
            {
                cachedInfo[point.X, point.Y] = value;
            }
        }

        public bool InMap(int x, int y)
        {
            return x > -1 && x < Area.Width && y > -1 && y < Area.Height;
        }

        public void Setup()
        {
            for (int i = 0; i < Area.Width; i++)
            {
                for (int j = 0; j < Area.Height; j++)
                {
                    cachedInfo[i, j] = new TileDataCache(Main.tile[Area.X + i, Area.Y + j]);
                }
            }
        }

        public void ClearArea()
        {
            for (int i = Area.X; i < Area.X + Area.Width; i++)
            {
                for (int j = Area.Y; j < Area.Y + Area.Height; j++)
                {
                    Main.tile[i, j].ClearEverything();
                }
            }
        }
        public void ResetArea()
        {
            for (int i = 0; i < Area.Width; i++)
            {
                for (int j = 0; j < Area.Height; j++)
                {
                    cachedInfo[i, j].Set(Main.tile[i + Area.X, j + Area.Y]);
                }
            }
        }


        public TileMapCache Clone()
        {
            var tileMapCache = new TileMapCache(Area, new TileDataCache[Width, Height], WorldID);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    tileMapCache.cachedInfo[i, j] = cachedInfo[i, j];
                }
            }
            return tileMapCache;
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
            return new int[] { Area.X, Area.Y, Area.Width, Area.Height, };
        }

        public byte[] CompressTileArray()
        {
            int bytesUsedByTiles = Area.Width * Area.Height * 12;
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);
                writer.Write(SaveType);
                writer.Write((int)TileID.Count);
                writer.Write((int)WallID.Count);
                var tileConversionTable = new Dictionary<int, string>();
                for (int i = 0; i < Area.Width; i++)
                {
                    for (int j = 0; j < Area.Height; j++)
                    {
                        if (cachedInfo[i, j].Type.Type >= TileID.Count && !tileConversionTable.ContainsKey(cachedInfo[i, j].Type.Type))
                        {
                            tileConversionTable.Add(cachedInfo[i, j].Type.Type, TileLoader.GetTile(cachedInfo[i, j].Type.Type).FullName);
                        }
                    }
                }

                var wallConversionTable = new Dictionary<int, string>();
                for (int i = 0; i < Area.Width; i++)
                {
                    for (int j = 0; j < Area.Height; j++)
                    {
                        if (cachedInfo[i, j].Wall.Type >= WallID.Count && !wallConversionTable.ContainsKey(cachedInfo[i, j].Wall.Type))
                        {
                            wallConversionTable.Add(cachedInfo[i, j].Wall.Type, TileLoader.GetTile(cachedInfo[i, j].Wall.Type).FullName);
                        }
                    }
                }

                writer.Write(tileConversionTable.Count);
                foreach (var pair in tileConversionTable)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }

                writer.Write(wallConversionTable.Count);
                foreach (var pair in wallConversionTable)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }

                long position = writer.BaseStream.Position;
                writer.Write((long)0);

                for (int i = 0; i < Area.Width; i++)
                {
                    for (int j = 0; j < Area.Height; j++)
                    {
                        writer.Write(cachedInfo[i, j].Type.Type);
                        writer.Write(cachedInfo[i, j].Liquid.Amount);
                        writer.Write(TileDataCache.TileReflectionHelper.LiquidData_typeAndFlags.GetValue<byte>(cachedInfo[i, j].Liquid));
                        writer.Write(cachedInfo[i, j].Misc.TileFrameX);
                        writer.Write(cachedInfo[i, j].Misc.TileFrameY);
                        writer.Write(TileDataCache.TileReflectionHelper.TileWallWireStateData_bitpack.GetValue<int>(cachedInfo[i, j].Misc));
                        writer.Write(cachedInfo[i, j].Wall.Type);
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
            //return new TileDataCache[width, height];

            using (var reader = new BinaryReader(new MemoryStream(buffer)))
            {
                var info = new TileDataCache[width, height];
                int type = reader.ReadInt32();
                int maxTiles = reader.ReadInt32();
                int maxWalls = reader.ReadInt32();

                int tileConversionCount = reader.ReadInt32();
                var tileConversionTable = new Dictionary<int, int>();

                for (int i = 0; i < tileConversionCount; i++)
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
                    tileConversionTable.Add(tileConversionKey, tileResultID);
                }

                int wallConversionCount = reader.ReadInt32();
                var wallConversionTable = new Dictionary<int, int>();

                for (int i = 0; i < wallConversionCount; i++)
                {
                    int wallResultID = -1;
                    int wallConversionID = reader.ReadInt32();
                    var wallKey = reader.ReadString();
                    var split = wallKey.Split('/');
                    if (ModLoader.TryGetMod(split[0], out var mod))
                    {
                        if (mod.TryFind<ModWall>(split[1], out var modWall))
                        {
                            wallResultID = modWall.Type;
                            if (wallResultID == wallConversionID)
                                continue;
                        }
                    }
                    wallConversionTable.Add(wallConversionID, wallResultID);
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
                        if (tileType.Type >= maxTiles && tileConversionTable.TryGetValue(tileType.Type, out int val))
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
                        var wall = new WallTypeData()
                        {
                            Type = reader.ReadUInt16(),
                        };
                        if (wall.Type >= maxTiles && wallConversionTable.TryGetValue(wall.Type, out int wallVal))
                        {
                            wall.Type = (ushort)wallVal;
                        }
                        info[i, j] = new TileDataCache(tileType, liquid, misc, wall);
                    }
                }
                return info;
            }
        }
    }
}