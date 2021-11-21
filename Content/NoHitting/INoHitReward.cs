using Terraria;

namespace AQMod.Content.NoHitting
{
    public interface INoHitReward
    {
        bool OnEffect(NPC npc, int hitDirection, double damage, NoHitNPC noHitManager);
        void NPCLoot(NPC npc, NoHitNPC noHitManager);
    }
}