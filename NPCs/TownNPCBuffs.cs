using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using AQMod.Common;

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