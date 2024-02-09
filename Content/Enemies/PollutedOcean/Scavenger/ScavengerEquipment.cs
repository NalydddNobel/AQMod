using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public partial class ScavengerEquipment : MetadataSet {
    [JsonProperty]
    public static List<Entry<ItemID>> ScavengerHelmets { get; private set; } = new();
    [JsonProperty]
    public static List<Entry<ItemID>> ScavengerBreastplates { get; private set; } = new();
    [JsonProperty]
    public static List<Entry<ItemID>> ScavengerLeggings { get; private set; } = new();
    [JsonProperty]
    public static List<Entry<ItemID>> ScavengerAccessories { get; private set; } = new();
    [JsonProperty]
    public static List<Entry<ItemID>> ScavengerWeapons { get; private set; } = new();
}