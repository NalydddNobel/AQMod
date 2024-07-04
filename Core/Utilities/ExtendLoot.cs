using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.GameContent.ItemDropRules;
using Terraria.Utilities;

namespace AequusRemake.Core.Utilities;

public static class ExtendLoot {
    private static readonly MethodInfo _resolveRule = typeof(ItemDropResolver).GetMethod("ResolveRule", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    public static ItemDropAttemptResult ResolveRule(IItemDropRule rule, in DropAttemptInfo info) {
        return (ItemDropAttemptResult)_resolveRule.Invoke(Main.ItemDropSolver, [rule, info]);
    }

    /// <param name="npc"></param>
    /// <param name="player"></param>
    /// <param name="rng">RNG Override. Defaults to <see cref="Main.rand"/> when null.</param>
    /// <returns>A <see cref="DropAttemptInfo"/> value for the current NPC.</returns>
    public static DropAttemptInfo GetDropAttemptInfo(NPC npc, Player player, UnifiedRandom rng = null) {
        return new DropAttemptInfo() {
            IsExpertMode = Main.expertMode,
            IsMasterMode = Main.masterMode,
            npc = npc,
            player = player,
            rng = rng ?? Main.rand,
            IsInSimulation = false
        };
    }

    public static void RemoveAll(this NPCLoot loot, Predicate<IItemDropRule> predicate) {
        foreach (IItemDropRule item in loot.Get(includeGlobalDrops: false)) {
            if (predicate(item)) {
                loot.Remove(item);
            }
        }
    }

    /// <summary>Inherits all of the Drop Rules from a specified NPC Id. (<paramref name="parentNPCId"/>)</summary>
    /// <param name="parentNPCId">The NPC Id to take the Drop Rules from.</param>
    /// <param name="childNPCId">The NPC Id to give the Drop Rules to.</param>
    /// <param name="database">Database instance. Set to null to use <see cref="Main.ItemDropsDB"/>.</param>
    public static void InheritDropRules(int parentNPCId, int childNPCId, ItemDropDatabase database = null) {
        List<IItemDropRule> drops = GetDropRules(parentNPCId, database);
        NPCLoot npcLoot = GetNPCLoot(childNPCId, database);
        foreach (var d in drops) {
            npcLoot.Add(d);
        }
    }

    /// <summary>Gets an <see cref="NPCLoot"/> from an NPC Id.</summary>
    /// <param name="npcId">The NPC Id.</param>
    /// <param name="database">Database instance. Set to null to use <see cref="Main.ItemDropsDB"/>.</param>
    public static NPCLoot GetNPCLoot(int npcId, ItemDropDatabase database = null) {
        return new NPCLoot(npcId, database ?? Main.ItemDropsDB);
    }

    /// <summary>Gets Drop Rules registered to the specified NPC Id.</summary>
    /// <param name="npcId">The NPC Id.</param>
    /// <param name="database">Database instance. Set to null to use <see cref="Main.ItemDropsDB"/>.</param>
    public static List<IItemDropRule> GetDropRules(int npcId, ItemDropDatabase database = null) {
        return (database ?? Main.ItemDropsDB).GetRulesForNPCID(npcId, includeGlobalDrops: false);
    }
}
