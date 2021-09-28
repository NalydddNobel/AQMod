using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.IO
{
    public class ModItemIO : ModContentIO<Item>
    {
        public override string GetKey(int type)
        {
            var item = new Item();
            item.SetDefaults(type);
            return GetKey(item);
        }

        public override string GetKey(Item value)
        {
            if (value.type < Main.maxNPCs)
            {
                return "0:" + value.type + ";";
            }
            return "1:" + value.modItem.mod.Name + ":" + value.modItem.Name + ";";
        }

        public override int GetID(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }
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
                    return -1;
                }
                else
                {
                    return itemType;
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
                    return -1;
                }
                else
                {
                    return itemType;
                }
            }
        }
    }
}