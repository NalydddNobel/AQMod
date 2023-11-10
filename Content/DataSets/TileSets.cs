using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.DataSets;

[DataID(typeof(TileID))]
public class TileSets : DataSet {
    public TileSets() : base() {
    }

    [JsonProperty]
    public static DataIDValueSet Mechanical;
    [JsonProperty]
    public static DataIDValueSet HasConsistentRNG;

    public override void PostSetupContent() {
        for (int i = 0; i < TileLoader.TileCount; i++) {
            if (TileID.Sets.Torch[i] || TileID.Sets.OpenDoorID[i] > -1 || TileID.Sets.CloseDoorID[i] > -1) {
                Mechanical.Add(i);
            }
        }
    }
}