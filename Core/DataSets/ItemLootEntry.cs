using Newtonsoft.Json;

namespace Aequus.Core.DataSets;

public struct ItemLootEntry {
    [JsonProperty]
    public ItemEntry Item { get; set; }
    [JsonProperty]
    public JsonRange Stack { get; set; }
    [JsonProperty]
    public PrefixEntry Prefix { get; set; }

    public ItemLootEntry(int id, int stack, int prefix) : this(id, stack, stack, prefix) { }
    public ItemLootEntry(int id, int min, int max, int prefix) {
        Item = new(id);
        Stack = new(min, max);
        Prefix = new(prefix);
    }
}
