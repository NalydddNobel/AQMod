using System;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Core.Utilities;

public static class ExtendLoot {
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
