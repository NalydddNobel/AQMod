using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus {
    public static partial class Helper
    {
        public static void ChestConversionNetUpdate(int chestID, int tileID, int x, int y)
        {
            int weirdThing = 1;
            if (tileID == TileID.Containers2)
            {
                weirdThing = 5;
            }
            if (tileID >= TileID.Count)
            {
                weirdThing = 101;
            }
            NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, weirdThing, x, y, 0f, chestID, Main.tile[x, y].TileType);
            NetMessage.SendTileSquare(-1, x, y, 3);
        }

        public static void ChestConversionNetUpdate(int chestID)
        {
            ChestConversionNetUpdate(chestID, Main.tile[Main.chest[chestID].x, Main.chest[chestID].y].TileType, Main.chest[chestID].x, Main.chest[chestID].y);
        }

        public static void WriteNPCIndex(this BinaryWriter writer, int value, byte invalid = 255) {
            writer.Write(value < 0 || value >= Main.maxNPCs ? invalid : (byte)value);
        }
        public static int ReadNPCIndex(this BinaryReader reader, byte invalidByte = 255, int invalidReturn = -1) {
            byte value = reader.ReadByte();
            return value == invalidByte ? invalidReturn : value;
        }
    }
}