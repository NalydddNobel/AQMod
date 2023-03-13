using Terraria;
using Terraria.ID;

namespace Aequus
{
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
    }
}