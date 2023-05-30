using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;

namespace Aequus.Tiles {
    public struct AequusTileData : ITileData
    {
        internal byte bitpack;

        public bool Uncuttable
        {
            get
            {
                return TileDataPacking.GetBit(bitpack, 0);
            }
            set
            {
                bitpack = (byte)TileDataPacking.SetBit(value, bitpack, 0);
            }
        }

        public void OnKillTile()
        {
            Uncuttable = false;
        }

        public static byte[] Save()
        {
            var baseStream = new MemoryStream();
            var writer = new BinaryWriter(baseStream);

            bool writeStart = true;
            long endPosition = writer.BaseStream.Position;
            for (int k = 0; k < Main.maxTilesX * Main.maxTilesY; k++)
            {
                int i = k % Main.maxTilesX;
                int j = k / Main.maxTilesX;

                var bitPack = Main.tile[i, j].Get<AequusTileData>().bitpack;
                if (bitPack == 0)
                {
                    if (!writeStart)
                    {
                        writer.Write((byte)0);
                        writeStart = true;
                    }
                    continue;
                }

                if (writeStart)
                {
                    writer.Write(k);
                    writeStart = false;
                }
                writer.Write(bitPack);
            }
            writer.Write(-1);
            var buf = baseStream.GetBuffer();
            writer.Dispose();
            return buf;
        }

        public static void Load(byte[] buf)
        {
            if (buf.Length == 0)
                return;

            using (var reader = new BinaryReader(new MemoryStream(buf)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int k = reader.ReadInt32();
                    if (k == -1)
                        break;
                    for (; ; k++)
                    {
                        var b = reader.ReadByte();
                        if (b == 0)
                            break;

                        int i = k % Main.maxTilesX;
                        int j = k / Main.maxTilesX;
                        Main.tile[i, j].Get<AequusTileData>().bitpack = b;
                    }
                }
            }
        }

        public static bool ShouldSendSquare(Rectangle rectangle)
        {
            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
            {
                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                {
                    if (Main.tile[i, j].Get<AequusTileData>().bitpack != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void SendSquare(BinaryWriter writer, Rectangle syncTangle)
        {
            writer.Write(syncTangle.X);
            writer.Write(syncTangle.Y);
            writer.Write(syncTangle.Width);
            writer.Write(syncTangle.Height);

            for (int i = syncTangle.X; i < syncTangle.X + syncTangle.Width; i++)
            {
                for (int j = syncTangle.Y; j < syncTangle.Y + syncTangle.Height; j++)
                {
                    var b = Main.tile[i, j].Get<AequusTileData>().bitpack;
                    writer.Write(b);
                }
            }
        }

        public static void ReadSquare(BinaryReader reader)
        {
            var r = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            if (Netplay.Clients[Main.myPlayer].SectionRange(Math.Max(r.Width, r.Height), r.X, r.Y))
            {
                for (int i = r.X; i < r.X + r.Width; i++)
                {
                    for (int j = r.Y; j < r.Y + r.Height; j++)
                    {
                        var val = reader.ReadByte();
                        try
                        {
                            Main.tile[i, j].Get<AequusTileData>().bitpack = val;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            else
            {
                reader.BaseStream.Position += r.Width * r.Height;
            }
        }
    }
}