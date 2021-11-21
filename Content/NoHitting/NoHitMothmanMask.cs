using AQMod.NPCs.Boss.Starite;
using Terraria;

namespace AQMod.Content.NoHitting
{
    public class NoHitMothmanMask : INoHitReward
    {
        public readonly int Type;
        public readonly int Stack;

        public NoHitMothmanMask(int type)
        {
            Type = type;
            Stack = 1;
        }

        public NoHitMothmanMask(int type, int stack)
        {
            Type = type;
            Stack = stack;
        }

        public void NPCLoot(NPC npc, NoHitManager noHitManager)
        {
            if (Main.eclipse && Main.dayTime)
                Item.NewItem(npc.getRect(), Type, Stack);
        }

        public bool OnEffect(NPC npc, int hitDirection, double damage, NoHitManager noHitManager)
        {
            return Main.eclipse && Main.dayTime;
        }
    }
}