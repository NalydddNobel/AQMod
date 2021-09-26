using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common
{
    internal class ChestTypeAndStyleToItemID
    {
        private static Dictionary<(ushort, byte), int> _chestTypeAndStyleToItemID;

        public static int Value(int tileID, int style)
        {
            return _chestTypeAndStyleToItemID[((ushort)tileID, (byte)style)];
        }

        /// <summary>
        /// If the dictionary doesn't contain this tile ID and style, returns -1. Otherwise it returns the item ID.
        /// </summary>
        /// <param name="tileID"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static int TryGetValue(int tileID, int style)
        {
            return _chestTypeAndStyleToItemID.ContainsKey(((ushort)tileID, (byte)style))
                ? _chestTypeAndStyleToItemID[((ushort)tileID, (byte)style)]
                : -1;
        }

        public static void Setup()
        {
            _chestTypeAndStyleToItemID = new Dictionary<(ushort, byte), int>();
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                var item = new Item();
                item.SetDefaults(i);
                if (item.createTile >= 0)
                {
                    if (Main.tileContainer[item.createTile])
                        _chestTypeAndStyleToItemID[((ushort)item.createTile, (byte)item.placeStyle)] = i;
                }
            }
        }

        public static void Unload()
        {
            _chestTypeAndStyleToItemID = null;
        }
    }
}