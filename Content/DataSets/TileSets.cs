using Aequus.Core.DataSets;
using Newtonsoft.Json;
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
    [JsonProperty]
    public static DataIDBoolArraySet IsSmashablePot;

    public override void PostSetupContent() {
        IsSmashablePot.CheckCapacity(TileLoader.TileCount);
        HasConsistentRNG.AddRange(IsSmashablePot);
        for (int i = 0; i < TileLoader.TileCount; i++) {
            if (TileID.Sets.Torch[i] || TileID.Sets.OpenDoorID[i] > -1 || TileID.Sets.CloseDoorID[i] > -1) {
                Mechanical.Add(i);
            }
        }
    }
}