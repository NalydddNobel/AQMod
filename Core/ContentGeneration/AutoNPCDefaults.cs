using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.ContentGeneration;

public sealed class AutoNPCDefaults : GlobalNPC {
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (npc.ModNPC == null) {
            return;
        }

        foreach (Attribute attr in npc.ModNPC.GetType().GetCustomAttributes()) {
            if (attr is AutoloadTrophiesAttribute) {
                npcLoot.Add(ItemDropRule.MasterModeCommonDrop(Mod.Find<ModItem>($"{npc.ModNPC.Name}Relic").Type));
                npcLoot.Add(ItemDropRule.Common(Mod.Find<ModItem>($"{npc.ModNPC.Name}Trophy").Type, chanceDenominator: 10));
            }
        }
    }
}
