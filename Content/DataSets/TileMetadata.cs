using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.DataSets;

public class TileMetadata : MetadataSet {
    [JsonProperty]
    public static HashSet<Entry<TileID>> IsSmashablePot { get; private set; } = new();
    /// <summary>Prevents tiles below this tile from being sloped.</summary>
    [JsonIgnore]
    public static HashSet<ushort> PreventsSlopesBelow { get; private set; } = new();
}