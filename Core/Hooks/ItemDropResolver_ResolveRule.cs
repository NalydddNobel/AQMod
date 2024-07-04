using System;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static bool _rerollingItemDropResolve;

    /// <summary>Allows for re-rolling item drops (if they fail an RNG roll) for the Grand Reward.</summary>
    private static ItemDropAttemptResult ItemDropResolver_ResolveRule(On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info) {
        ItemDropAttemptResult result = orig(self, rule, info);

#if !DEBUG
        if (_rerollingItemDropResolve || result.State != ItemDropAttemptResultState.FailedRandomRoll) {
            return result;
        }

        float rolls = 0f;
        if (info.player != null) {
            //rolls += info.player.GetModPlayer<AequusPlayer>().dropRolls;
        }

        //if (info.npc != null) {
        //    rolls += info.npc.GetGlobalNPC<AequusNPC>().dropRolls;
        //}

        do {
            _rerollingItemDropResolve = true;
            try {
                ItemDropAttemptResult nextResult = orig(self, rule, info);
                if (nextResult.State != ItemDropAttemptResultState.FailedRandomRoll) {
                    _rerollingItemDropResolve = false;
                    return nextResult;
                }
            }
            catch (Exception ex) {
                AequusRemake.Instance.Logger.Error(ex);
            }
            _rerollingItemDropResolve = false;
            rolls--;
        }
        while (rolls > 0f && info.rng.NextFloat(1f) < rolls);
#endif

        return result;
    }
}
