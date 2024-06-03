using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.DataSets;

public partial class TileDataSet {
    /// <summary>Allows the tile to have Polluted Ocean ambient tiles spawn on it.</summary>
    [JsonIgnore]
    public static HashSet<ushort> Polluted { get; private set; } = [];
    /// <summary>When this tile is nearby, it will contribute to <see cref="Content.Biomes.PollutedOcean.PollutedOceanSystem.PollutedTileCount"/>.</summary>
    [JsonIgnore]
    public static List<ushort> GivesPollutedBiomePresence { get; private set; } = [];
    /// <summary>When this tile is near a <see cref="Polluted"/> tile, it will be destroyed during world generation.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<TileID>> RemovedInPollutedOceanGen { get; private set; } = [];
}
