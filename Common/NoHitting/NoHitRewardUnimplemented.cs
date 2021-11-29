using Terraria;

namespace AQMod.Common.NoHitting
{
    public class NoHitRewardUnimplemented : INoHitReward
    {
        void INoHitReward.NPCLoot(NPC npc, NoHitManager noHitManager)
        {
        }

        bool INoHitReward.OnEffect(NPC npc, int hitDirection, double damage, NoHitManager noHitManager)
        {
            return false;
        }
    }
}