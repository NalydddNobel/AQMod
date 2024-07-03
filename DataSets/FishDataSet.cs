using Aequu2.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequu2.DataSets;

public class FishDataSet : DataSet {
    [JsonProperty]
    public static List<IDEntry<ItemID>> Corrupt { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> Crimson { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> Hallow { get; private set; } = new();
    /// <summary>Items in this set are fishing junk.</summary>
    [JsonProperty]
    public static List<IDEntry<ItemID>> Junk { get; private set; } = new();
}
