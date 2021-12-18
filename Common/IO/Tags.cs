using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common.IO
{
    internal sealed class Tags
    {
        public static class Item
        {
            public static TagCompound SaveItemID(int item)
            {
                if (item >= Main.maxItemTypes)
                {
                    var modItem = ItemLoader.GetItem(item);
                    return SaveItemID(modItem.mod.Name, modItem.Name);
                }
                else
                {
                    return new TagCompound()
                    {
                        ["id"] = item,
                    };
                }
            }

            public static TagCompound SaveItemID(string mod, string item)
            {
                return new TagCompound()
                {
                    ["mod"] = mod,
                    ["name"] = item,
                };
            }

            /// <summary>
            /// Returns 0 if the item doesn't exist.
            /// </summary>
            /// <param name="tag"></param>
            /// <returns></returns>
            public static int GetItemID(TagCompound tag)
            {
                if (tag == null)
                    return 0;
                if (tag.ContainsKey("id"))
                {
                    return tag.GetInt("id");
                }
                string modName = tag.GetString("mod");
                if (string.IsNullOrEmpty(modName))
                    return 0;
                try
                {
                    var mod = ModLoader.GetMod(modName);
                    if (mod != null)
                    {
                        return mod.ItemType(tag.GetString("name"));
                    }
                }
                catch
                {
                }
                return 0;
            }
        }
    }
}