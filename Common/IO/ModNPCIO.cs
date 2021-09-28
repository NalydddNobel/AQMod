using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.IO
{
    public class ModNPCIO : ModContentIO<NPC>
    {
        public override string GetKey(int type)
        {
            var npc = new NPC();
            npc.SetDefaults(type);
            return GetKey(npc);
        }

        public override string GetKey(NPC value)
        {
            if (value.type < Main.maxNPCs)
            {
                return "0:" + value.type + ";";
            }
            return "1:" + value.modNPC.mod.Name + ":" + value.modNPC.Name + ";";
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
                int npcType = int.Parse(id);
                if (npcType >= Main.maxNPCTypes)
                {
                    return -1;
                }
                else
                {
                    return npcType;
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
                int npcType = modInstance.NPCType(name);
                if (npcType < Main.maxNPCTypes)
                {
                    return -1;
                }
                else
                {
                    return npcType;
                }
            }
        }
    }
}