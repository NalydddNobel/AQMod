using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;

namespace Aequus.Tiles
{
    public struct CoatingData : ITileData
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
                        writer.Write(Main.tile[i, j].Get<CoatingData>().bitpack);
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
                        Main.tile[i, j].Get<CoatingData>().bitpack = reader.ReadByte();
                    }
                }
            }
        }
    }
}