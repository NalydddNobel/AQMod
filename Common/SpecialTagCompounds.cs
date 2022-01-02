using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Common
{
    internal static class SpecialTagCompounds
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
        public static class NPC
        {
            public static TagCompound SaveNPCID(int npc)
            {
                if (npc >= Main.maxNPCTypes)
                {
                    var ModNPC = NPCLoader.GetNPC(npc);
                    return SaveNPCID(ModNPC.mod.Name, ModNPC.Name);
                }
                else
                {
                    return new TagCompound()
                    {
                        ["id"] = npc,
                    };
                }
            }

            public static TagCompound SaveNPCID(string mod, string item)
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
            public static int GetNPCID(TagCompound tag)
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
                        return mod.NPCType(tag.GetString("name"));
                    }
                }
                catch
                {
                }
                return 0;
            }
        }

        internal static Dictionary<string, object> test_RipOutTagData(this TagCompound tag)
        {
            return (Dictionary<string, object>)typeof(TagCompound).GetField("dict", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(tag);
        }
    }
}