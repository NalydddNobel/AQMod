using System.Reflection;
using Terraria;

namespace AQMod.NPCs
{
    internal static class NPCUtils
    {
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
