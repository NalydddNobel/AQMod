using Aequus.Core.DataSets;
using Newtonsoft.Json;
using Terraria.ID;

namespace Aequus.Content.DataSets;

public partial class NPCSets /* Scavenger */ {
    [JsonProperty]
    [DataID(typeof(ItemID))]
    public static DataIDValueSet ScavengerHelmets;
    [JsonProperty]
    [DataID(typeof(ItemID))]
    public static DataIDValueSet ScavengerBreastplates;
    [JsonProperty]
    [DataID(typeof(ItemID))]
    public static DataIDValueSet ScavengerLeggings;
    [JsonProperty]
    [DataID(typeof(ItemID))]
    public static DataIDValueSet ScavengerAccessories;
    [JsonProperty]
    [DataID(typeof(ItemID))]
    public static DataIDValueSet ScavengerWeapons;
}