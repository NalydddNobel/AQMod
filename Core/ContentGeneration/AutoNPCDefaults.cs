﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.ContentGeneration;

public sealed class AutoNPCDefaults : GlobalNPC {
    internal static readonly Dictionary<int, short> _npcToCritter = new();
    internal static readonly Dictionary<ModNPC, ModItem> _npcToBossBag = new();

    public override void Unload() {
        _npcToCritter.Clear();
        _npcToBossBag.Clear();
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
                npcLoot.Add(ItemDropRule.BossBag(_npcToBossBag[modNPC].Type));
            }
        }
    }
}