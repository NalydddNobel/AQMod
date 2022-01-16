using AQMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.NPCs
{
    public class TownNPCBuffs : GlobalNPC
    {
        public override void AI(NPC npc)
        {
            if (npc.townNPC || npc.type == NPCID.SkeletonMerchant)
            {
                if (WorldDefeats.TownNPCLavaImmunity)
                {
                    npc.lavaImmune = true;
                }
            }
        }
    }
}