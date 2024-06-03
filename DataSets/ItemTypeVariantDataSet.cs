using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.DataSets;

/// <summary>Dataset with lists of all supported 'alts' of an item.</summary>
public class ItemTypeVariantDataSet : DataSet {
    [JsonProperty]
    public static List<IDEntry<ItemID>> RottenChunk { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> DemoniteBar { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> CopperBar { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> CopperHelmet { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> CopperChainmail { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> CopperGreaves { get; private set; } = new();
}
