using AQMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.TagItems.Starbyte
{
    public struct StarbytePotionTag
    {
        public readonly ushort potionItemID;
        public ushort buffID { get; private set; }
        public uint buffTime { get; private set; }

        public static StarbytePotionTag Default => new StarbytePotionTag(ItemID.ObsidianSkinPotion, BuffID.ObsidianSkin, 6000);

        public static StarbytePotionTag Error => new StarbytePotionTag(ItemID.ObsidianSkinPotion, ushort.MaxValue, 6000);

        internal StarbytePotionTag(int item, int buffID, uint buffTime)
        {
            potionItemID = (ushort)item;
            this.buffID = (ushort)buffID;
            this.buffTime = buffTime;
        }

        public StarbytePotionTag(int item)
        {
            potionItemID = (ushort)item;
            buffID = 0;
            buffTime = 0;
            SetupTag();
        }

        public static StarbytePotionTag FromKey(string key)
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
                        return new StarbytePotionTag(itemType);
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
                        return new StarbytePotionTag(itemType);
                    }
                }
            }
            catch
            {
                return Error;
            }
        }

        public string GetKey()
        {
            if (potionItemID < Main.maxItemTypes)
            {
                return "0:" + potionItemID + ";";
            }
            else
            {
                var item = new Item();
                item.netDefaults(potionItemID);
                return "1:" + item.modItem.mod.Name + ":" + item.modItem.Name + ";";
            }
        }

        public bool SetupTag()
        {
            var item = new Item();
            item.netDefaults(potionItemID);
            if (item.buffType <= 0 || item.buffTime <= 0)
            {
                return false;
            }
            buffID = (ushort)item.buffType;
            buffTime = (uint)(item.buffTime * 2);
            return true;
        }
    }
}