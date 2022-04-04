using Aequus.Items.Weapons.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public sealed class AequusNPCDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.UndeadViking || npc.type == NPCID.ArmoredViking)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalDagger>(), 12));
            }
        }
    }
}