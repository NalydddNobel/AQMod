using Terraria;

namespace AQMod.Content.NoHitting
{
    public class NoHitOmegaStarite : INoHitReward
    {
        public readonly int Type;
        public readonly int Stack;

        public NoHitOmegaStarite(int type)
        {
            Type = type;
            Stack = 1;
        }

        public NoHitOmegaStarite(int type, int stack)
        {
            Type = type;
            Stack = stack;
        }

        public void NPCLoot(NPC npc, NoHitManager noHitManager)
        {
            Item.NewItem(npc.getRect(), Type, Stack);
        }

        public bool OnEffect(NPC npc, int hitDirection, double damage, NoHitManager noHitManager)
        {
            return npc.lifeMax == -33333;
        }
    }
}