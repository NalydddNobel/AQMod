using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.NPCs;

public sealed class DropsGlobalNPC : GlobalNPC {
    private static readonly Dictionary<int, List<IItemDropRule>> _dropRules = new();

    public bool noOnKillEffects;

    public override bool InstancePerEntity => true;

    public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
        globalLoot.Add(Content.PermaPowerups.NoHit.NoHitReward.GetGlobalLoot());
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (_dropRules.TryGetValue(npc.netID, out var dropRules)) {
            foreach (var rule in dropRules) {
                npcLoot.Add(rule);
            }
        }
    }

    public override bool SpecialOnKill(NPC npc) {
        if (noOnKillEffects) {
            return true;
        }

        return base.SpecialOnKill(npc);
    }


    /// <summary>
    /// Allows you to add a drop rule to an NPC. Please only call this in SetStaticDefaults/PostSetupContent.
    /// </summary>
    /// <param name="npcId">The NPC type.</param>
    /// <param name="rule">The item drop rule.</param>
    internal static void AddNPCLoot(int npcId, IItemDropRule rule) {
        (CollectionsMarshal.GetValueRefOrAddDefault(_dropRules, npcId, out _) ??= new()).Add(rule);
    }

    public override void Load() {
    }

    public override void Unload() {
        _dropRules.Clear();
    }
}
