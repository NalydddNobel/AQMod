using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.ContentGeneration;

internal sealed class AutoNPCDefaults : GlobalNPC {
    internal static readonly Dictionary<int, short> _npcToCritter = new();

    public override void Unload() {
        _npcToCritter.Clear();
    }

    public override void SetDefaults(NPC npc) {
        if (_npcToCritter.TryGetValue(npc.type, out short critter)) {
            npc.catchItem = critter;
        }
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (npc.ModNPC == null) {
            return;
        }

        ModNPC modNPC = ModContent.GetModNPC(npc.type);
        foreach (Attribute attr in modNPC.GetType().GetCustomAttributes()) {
            if (attr is AutoloadTrophiesAttribute) {
                npcLoot.Add(ItemDropRule.MasterModeCommonDrop(Mod.Find<ModItem>($"{modNPC.Name}Relic").Type));
                npcLoot.Add(ItemDropRule.Common(Mod.Find<ModItem>($"{modNPC.Name}Trophy").Type, chanceDenominator: 10));
            }

            if (attr is AutoloadBossBagAttribute) {
                npcLoot.Add(ItemDropRule.BossBag(Mod.Find<ModItem>($"{modNPC.Name}Bag").Type));
            }

            if (attr is AutoloadBossMaskAttribute) {
                npcLoot.AddBossDrop(ItemDropRule.Common(Mod.Find<ModItem>($"{modNPC.Name}Mask").Type, chanceDenominator: 7), throwError: false);
            }
        }
    }
}
