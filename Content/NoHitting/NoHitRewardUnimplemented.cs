using Terraria;

namespace AQMod.Content.NoHitting
{
    public class NoHitRewardUnimplemented : INoHitReward
    {
        void INoHitReward.NPCLoot(NPC npc, NoHitNPC noHitManager)
        {
        }

        bool INoHitReward.OnEffect(NPC npc, int hitDirection, double damage, NoHitNPC noHitManager)
        {
            return false;
        }
    }
}