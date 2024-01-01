using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.DataSets;

public class TileSets : DataSet {
    public TileSets() : base() {
    }

    [JsonProperty]
    public static HashSet<TileEntry> Mechanical { get; private set; } = new();
    [JsonProperty]
    public static HashSet<TileEntry> IsSmashablePot { get; private set; } = new();

    public override void PostSetupContent() {
        for (int i = 0; i < TileLoader.TileCount; i++) {
            if (TileID.Sets.Torch[i] || TileID.Sets.OpenDoorID[i] > -1 || TileID.Sets.CloseDoorID[i] > -1) {
                Mechanical.Add((TileEntry)i);
            }
        }
    }
}