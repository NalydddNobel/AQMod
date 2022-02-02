using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    internal static class NPCUtilities
    {
        public static class IO
        {
            public static string GetSerializationKey(int type)
            {
                var npc = new NPC();
                npc.SetDefaults(type);
                return GetSerializationKey(npc);
            }

            public static string GetSerializationKey(NPC value)
            {
                if (value.type < Main.maxNPCs)
                {
                    return "0:" + value.type + ";";
                }
                return "1:" + value.modNPC.mod.Name + ":" + value.modNPC.Name + ";";
            }

            public static int DeserializeKey(string key)
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

        public static void TargetClosest_UpdateToFaceTowardsUndetectableTargetsToo(this NPC npc)
        {
            npc.TargetClosest(faceTarget: true);
            if (npc.target != -1 && Main.player[npc.target].active && Main.player[npc.target].dead && Main.player[npc.target].GetModPlayer<AQPlayer>().undetectable)
            {
                npc.direction = Main.player[npc.target].position.X + Main.player[npc.target].width / 2f < npc.position.X + npc.width / 2f ? -1 : 1;
            }
        }

        public static bool IsntFriendly(this NPC npc)
        {
            return npc.active && npc.lifeMax > 5 && !npc.friendly && !npc.townNPC;
        }

        public static void SetLiquidSpeed(this NPC npc, float water = 0.5f, float lava = 0.5f, float honey = 0.25f)
        {
            typeof(NPC).GetField("waterMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, water);
            typeof(NPC).GetField("lavaMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, lava);
            typeof(NPC).GetField("honeyMovementSpeed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(npc, honey);
        }
    }
}
