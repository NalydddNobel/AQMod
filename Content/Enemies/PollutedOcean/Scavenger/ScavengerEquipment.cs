using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.Enemies.PollutedOcean.Scavenger;

public partial class ScavengerEquipment : DataSet {
    [JsonProperty]
    public static List<ItemEntry> ScavengerHelmets { get; private set; } = new();
    [JsonProperty]
    public static List<ItemEntry> ScavengerBreastplates { get; private set; } = new();
    [JsonProperty]
    public static List<ItemEntry> ScavengerLeggings { get; private set; } = new();
    [JsonProperty]
    public static List<ItemEntry> ScavengerAccessories { get; private set; } = new();
    [JsonProperty]
    public static List<ItemEntry> ScavengerWeapons { get; private set; } = new();
}