using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace Aequus.Tiles
{
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

        public static void SendSquares(BinaryWriter writer, List<Rectangle> syncTangles)
        {
            writer.Write(syncTangles.Count);
            for (int k = 0; k < syncTangles.Count; k++)
            {
                var r = syncTangles[k];
                writer.Write(r.X);
                writer.Write(r.Y);
                writer.Write(r.Width);
                writer.Write(r.Height);

                for (int i = r.X; i < r.X + r.Width; i++)
                {
                    for (int j = r.Y; j < r.Y + r.Height; j++)
                    {
                        writer.Write(Main.tile[i, j].Get<AequusTileData>().bitpack);
                    }
                }
            }
        }

		public static void ReadSquares(BinaryReader reader)
        {
            int amt = reader.ReadInt32();
            for (int k = 0; k < amt; k++)
            {
                var r = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                for (int i = r.X; i < r.X + r.Width; i++)
                {
                    for (int j = r.Y; j < r.Y + r.Height; j++)
                    {
                        Main.tile[i, j].Get<AequusTileData>().bitpack = reader.ReadByte();
                    }
                }
            }
        }
    }
}