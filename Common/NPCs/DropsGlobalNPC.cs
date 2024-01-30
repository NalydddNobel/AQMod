using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.NPCs;
public sealed class DropsGlobalNPC : GlobalNPC {
    private static Boolean _rerolling;

    private static readonly Dictionary<Int32, List<IItemDropRule>> _dropRules = new();

    /// <summary>
    /// Allows you to add a drop rule to an NPC. Please only call this in SetStaticDefaults/PostSetupContent.
    /// </summary>
    /// <param name="npcId">The NPC type.</param>
    /// <param name="rule">The item drop rule.</param>
    internal static void AddNPCLoot(Int32 npcId, IItemDropRule rule) {
        (CollectionsMarshal.GetValueRefOrAddDefault(_dropRules, npcId, out _) ??= new()).Add(rule);
    }

    public override void Load() {
        On_NPC.NPCLoot_DropMoney += NPC_NPCLoot_DropMoney;
        On_ItemDropResolver.ResolveRule += ItemDropResolver_ResolveRule;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        if (_dropRules.TryGetValue(npc.netID, out var dropRules)) {
            foreach (var rule in dropRules) {
                npcLoot.Add(rule);
            }
        }
    }

    public override void Unload() {
        _dropRules.Clear();
    }

    #region Hooks
    private static void NPC_NPCLoot_DropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer) {
        if (closestPlayer.TryGetModPlayer<AequusPlayer>(out var aequusPlayer) && aequusPlayer.accGrandRewardDownside) {
            return;
        }
        orig(self, closestPlayer);
    }

    private static ItemDropAttemptResult ItemDropResolver_ResolveRule(On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info) {
        var result = orig(self, rule, info);
        if (_rerolling || result.State != ItemDropAttemptResultState.FailedRandomRoll) {
            return result;
        }

        Single rolls = 0f;
        if (info.player != null) {
            rolls += info.player.GetModPlayer<AequusPlayer>().dropRolls;
        }
        //if (info.npc != null) {
        //    rolls += info.npc.GetGlobalNPC<AequusNPC>().dropRolls;
        //}

        do {
            _rerolling = true;
            try {
                var result2 = orig(self, rule, info);
                if (result2.State != ItemDropAttemptResultState.FailedRandomRoll) {
                    _rerolling = false;
                    return result2;
                }
            }
            catch (Exception ex) {
                Aequus.Instance.Logger.Error(ex);
            }
            _rerolling = false;
            rolls--;
        }
        while (rolls > 0f && info.rng.NextFloat(1f) < rolls);

        return result;
    }
    #endregion
}
