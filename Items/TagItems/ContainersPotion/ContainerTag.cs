using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.TagItems.ContainersPotion
{
    /// <summary>
    /// Links the Item ID of a chest item to a Tile ID of a chest tile
    /// </summary>
    public struct ContainerTag
    {
        public readonly ushort chestItemID;
        public ushort chestTileID { get; private set; }
        public byte chestTileStyle { get; private set; }

        /// <summary>
        /// A container tag that is linked to a wooden chest
        /// </summary>
        public static ContainerTag Default => new ContainerTag(ItemID.Chest, TileID.Containers, 0);

        /// <summary>
        /// A container tag that is linked to a golden chest, given to container tags that don't load properly
        /// </summary>
        public static ContainerTag Error => new ContainerTag(ItemID.GoldChest, TileID.Containers, byte.MaxValue);

        public static ContainerTag FromKey(string key)
        {
            try
            {
                if (key[0] == '0')
                {
                    string id = "";
                    for (int i = 2; i < key.Length - 1 && key[i] != ':' && key[i] != ';'; i++)
                    {
                        id += key[i];
                    }
                    int itemType = int.Parse(id);
                    if (itemType >= Main.maxItemTypes)
                    {
                        return Error;
                    }
                    else
                    {
                        return new ContainerTag(itemType);
                    }
                }
                else
                {
                    string mod = "";
                    int cursor = 2;
                    for (; cursor < key.Length - 1 && key[cursor] != ':'; cursor++)
                    {
                        mod += key[cursor];
                    }
                    cursor++;
                    string name = "";
                    for (; cursor < key.Length - 1 && key[cursor] != ':' && key[cursor] != ';'; cursor++)
                    {
                        name += key[cursor];
                    }
                    var modInstance = ModLoader.GetMod(mod);
                    int itemType = modInstance.ItemType(name);
                    if (itemType < Main.maxItemTypes)
                    {
                        return Error;
                    }
                    else
                    {
                        return new ContainerTag(itemType);
                    }
                }
            }
            catch
            {
                return Error;
            }
        }

        internal ContainerTag(int item, int tileType, int tileStyle)
        {
            chestItemID = (ushort)item;
            chestTileID = (ushort)tileType;
            chestTileStyle = (byte)tileStyle;
        }

        public ContainerTag(int item)
        {
            chestItemID = (ushort)item;
            chestTileID = TileID.Containers;
            chestTileStyle = byte.MaxValue;
            SetupTag();
        }

        public string GetKey()
        {
            if (chestItemID < Main.maxItemTypes)
            {
                return "0:" + chestItemID + ";";
            }
            else
            {
                var item = new Item();
                item.netDefaults(chestItemID);
                return "1:" + item.modItem.mod.Name + ":" + item.modItem.Name + ";";
            }
        }

        public bool SetupTag()
        {
            var item = new Item();
            item.netDefaults(chestItemID);
            if (!Main.tileContainer[chestTileID] || Main.tileTable[chestTileID])
            {
                return false;
            }
            chestTileID = (ushort)item.createTile;
            chestTileStyle = (byte)item.placeStyle;
            return true;
        }
    }
}