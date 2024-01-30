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
    /// <summary>
    /// Prevents tiles below this tile from being sloped.
    /// </summary>
    [JsonIgnore]
    public static HashSet<System.UInt16> PreventsSlopesBelow { get; private set; } = new();

    public override void PostSetupContent() {
        for (System.Int32 i = 0; i < TileLoader.TileCount; i++) {
            if (TileID.Sets.Torch[i] || TileID.Sets.OpenDoorID[i] > -1 || TileID.Sets.CloseDoorID[i] > -1) {
                Mechanical.Add((TileEntry)i);
            }
        }
    }
}