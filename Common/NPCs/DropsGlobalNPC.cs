using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs;
public class DropsGlobalNPC : GlobalNPC {
    private static bool _rerolling;

    public override void Load() {
        On_NPC.NPCLoot_DropMoney += NPC_NPCLoot_DropMoney;
        On_ItemDropResolver.ResolveRule += ItemDropResolver_ResolveRule;
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

        float rolls = 0f;
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
