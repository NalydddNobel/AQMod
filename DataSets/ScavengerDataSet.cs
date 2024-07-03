using Aequu2.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequu2.DataSets;

public partial class ScavengerDataSet : DataSet {
    [JsonProperty]
    public static List<IDEntry<ItemID>> ScavengerHelmets { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> ScavengerBreastplates { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> ScavengerLeggings { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> ScavengerAccessories { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> ScavengerWeapons { get; private set; } = new();
}