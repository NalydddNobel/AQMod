using Aequus.DataSets.Structures;
using Aequus.DataSets.Structures.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.DataSets;

public class TileDataSet : DataSet {
    [JsonProperty]
    public static Dictionary<ChestStyle, Biome> ChestToBiome { get; private set; } = new();
    [JsonProperty]
    public static HashSet<IDEntry<TileID>> PhysicsGunCannotPickUp { get; private set; } = new();
    [JsonProperty]
    public static HashSet<IDEntry<TileID>> PhysicsGunBlocksLaser { get; private set; } = new();
    [JsonProperty]
    public static HashSet<IDEntry<TileID>> IsSmashablePot { get; private set; } = new();
    [JsonProperty]
    public static Dictionary<IDEntry<TileID>, Dictionary<int, Color>> PylonColors { get; private set; } = new();

    [JsonIgnore]
    public static HashSet<ushort> HasAnyConversions { get; private set; } = new();

    /// <summary>Prevents tiles below this tile from being sloped.</summary>
    [JsonIgnore]
    public static HashSet<ushort> PreventsSlopesBelow { get; private set; } = new();

    [JsonIgnore]
    public static bool[] All { get; private set; }

    public override void PostSetupContent() {
        All = ExtendArray.CreateArray((i) => true, TileLoader.TileCount);
    }

    public override void AddRecipes() {
        for (ushort i = 0; i < (ushort)TileLoader.TileCount; i++) {
            if (TileID.Sets.Conversion.Dirt[i]
                || TileID.Sets.Conversion.GolfGrass[i]
                || TileID.Sets.Conversion.Grass[i]
                || TileID.Sets.Conversion.HardenedSand[i]
                || TileID.Sets.Conversion.Ice[i]
                || TileID.Sets.Conversion.JungleGrass[i]
                || TileID.Sets.Conversion.Moss[i]
                || TileID.Sets.Conversion.MushroomGrass[i]
                || TileID.Sets.Conversion.Sand[i]
                || TileID.Sets.Conversion.Sandstone[i]
                || TileID.Sets.Conversion.Snow[i]
                || TileID.Sets.Conversion.Stone[i]
                || TileID.Sets.Conversion.Thorn[i]) {
                HasAnyConversions.Add(i);
            }
        }
    }
}