using Aequus.Core.DataSets;
using Aequus.Core.Initialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Aequus.Content.DataSets;

public struct LootDefinition {
    [JsonProperty]
    public ItemLootEntry PrimaryItem { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)] // Do not show this value if it's null
    public List<ItemLootEntry> SecondaryItems { get; set; }

    public static LootDefinition Create(int primaryItem, int stack = 1, params ItemLootEntry[] secondaryItems) {
        var def = new LootDefinition {
            PrimaryItem = new(primaryItem, stack, -1)
        };
        if (secondaryItems != null) {
            def.SecondaryItems = secondaryItems.ToList();
        }

        return def;
    }

    internal static void CreateFor(List<LootDefinition> lootPool, int primaryItem, int stack = 1, params ItemLootEntry[] secondaryItems) {
        Aequus.OnPostSetupContent += () => lootPool.Add(Create(primaryItem, stack, secondaryItems));
    }
}

public class Loot {
    public static readonly List<LootDefinition> PollutedOceanPrimary = new();
}