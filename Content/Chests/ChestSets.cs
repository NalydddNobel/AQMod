using Aequus.Common;
using Aequus.Systems.Chests;
using System.Collections.Generic;

namespace Aequus.Content.Chests;

public class ChestSets : LoadedType {
    public static ChestSets Instance => ModContent.GetInstance<ChestSets>();

    public readonly Dictionary<TileKey, HardmodeChestInfo> HardmodeConvert = [];
    public readonly List<ChestEnvironmentInfo> ChestEnvironment = [];

    public override void PostSetupContent() {
        ChestEnvironment.Add(new ChestEnvironmentInfo((i, j) => TileID.Sets.JungleBiome[Main.tile[i, j].TileType] > 0, ChestID.Ivy));
        ChestEnvironment.Add(new ChestEnvironmentInfo((i, j) => TileID.Sets.SnowBiome[Main.tile[i, j].TileType] > 0, ChestID.Frozen));
        ChestEnvironment.Add(new ChestEnvironmentInfo((i, j) => TileID.Sets.SandBiome[Main.tile[i, j].TileType] > 0, ChestID.Sandstone));
        ChestEnvironment.Add(new ChestEnvironmentInfo((i, j) => WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[Main.tile[i, j].WallType], ChestID.Sandstone));
        ChestEnvironment.Add(new ChestEnvironmentInfo(TileHelper.HasWallAction(WallID.SpiderUnsafe), ChestID.Webbed));
        ChestEnvironment.Add(new ChestEnvironmentInfo((i, j) => TileID.Sets.MushroomBiome[Main.tile[i, j].TileType] > 0, ChestID.Mushroom));
        ChestEnvironment.Add(new ChestEnvironmentInfo(TileHelper.HasWallAction(WallID.MarbleUnsafe, WallID.Marble, WallID.MarbleBlock), ChestID.Marble));
        ChestEnvironment.Add(new ChestEnvironmentInfo(TileHelper.HasWallAction(WallID.GraniteUnsafe, WallID.Granite, WallID.GraniteBlock), ChestID.Granite));
    }
}

public readonly record struct HardmodeChestInfo(TileKey ConvertType, ChestPool Loot);

public readonly record struct ChestEnvironmentInfo(Utils.TileActionAttempt ValidEnvionrment, TileKey Chest);