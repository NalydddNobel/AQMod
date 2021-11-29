using Terraria;

namespace AQMod.Common.NoHitting
{
    public class NoHitMothmanMask : INoHitReward
    {
        public readonly int Type;
        public readonly int Type2;
        public NoHitMothmanMask(int type, int type2)
        {
            Type = type;
            Type2 = type2;
        }

        public void NPCLoot(NPC npc, NoHitManager noHitManager)
        {
            if (Main.eclipse && Main.dayTime)
                Item.NewItem(npc.getRect(), Type);
            else
                Item.NewItem(npc.getRect(), Type2);
        }

        public bool OnEffect(NPC npc, int hitDirection, double damage, NoHitManager noHitManager)
        {
            return true;
        }
    }
}