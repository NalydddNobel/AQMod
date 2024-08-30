using Aequus.Common;
using Aequus.Systems.Chests;
using Aequus.Tiles.Furniture.HardmodeChests;
using System.Collections.Generic;

namespace Aequus.Content.Chests;

public class ChestSets : LoadedType {
    public static ChestSets Instance => ModContent.GetInstance<ChestSets>();

    public readonly record struct HardmodeInfo(TileKey ConvertType, ChestPool Loot);
    public readonly record struct EnvironmentInfo(Utils.TileActionAttempt ValidEnvionrment, TileKey Chest);

    public readonly Dictionary<TileKey, HardmodeInfo> HardmodeConvert = [];
    public readonly List<EnvironmentInfo> ChestEnvironment = [];

    protected override void PostSetupContent() {
        ChestEnvironment.Add(new EnvironmentInfo((i, j) => TileID.Sets.JungleBiome[Main.tile[i, j].TileType] > 0, ChestID.Ivy));
        ChestEnvironment.Add(new EnvironmentInfo((i, j) => TileID.Sets.SnowBiome[Main.tile[i, j].TileType] > 0, ChestID.Frozen));
        ChestEnvironment.Add(new EnvironmentInfo((i, j) => TileID.Sets.SandBiome[Main.tile[i, j].TileType] > 0, ChestID.Sandstone));
        ChestEnvironment.Add(new EnvironmentInfo((i, j) => WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn[Main.tile[i, j].WallType], ChestID.Sandstone));
        ChestEnvironment.Add(new EnvironmentInfo(TileHelper.HasWallAction(WallID.SpiderUnsafe), ChestID.Webbed));
        ChestEnvironment.Add(new EnvironmentInfo((i, j) => TileID.Sets.MushroomBiome[Main.tile[i, j].TileType] > 0, ChestID.Mushroom));
        ChestEnvironment.Add(new EnvironmentInfo(TileHelper.HasWallAction(WallID.MarbleUnsafe, WallID.Marble, WallID.MarbleBlock), ChestID.Marble));
        ChestEnvironment.Add(new EnvironmentInfo(TileHelper.HasWallAction(WallID.GraniteUnsafe, WallID.Granite, WallID.GraniteBlock), ChestID.Granite));

        HardmodeConvert[ChestID.Gold] = new HardmodeInfo((TileKey)ModContent.TileType<AdamantiteChestTile>(), ChestPool.UndergroundHard);
        HardmodeConvert[ChestID.Frozen] = new HardmodeInfo((TileKey)ModContent.TileType<HardFrozenChestTile>(), ChestPool.SnowHard);
        HardmodeConvert[ChestID.Granite] = new HardmodeInfo((TileKey)ModContent.TileType<HardGraniteChestTile>(), ChestPool.UndergroundHard);
        HardmodeConvert[ChestID.RichMahogany] = new HardmodeInfo((TileKey)ModContent.TileType<HardJungleChestTile>(), ChestPool.JungleHard);
        HardmodeConvert[ChestID.Ivy] = new HardmodeInfo((TileKey)ModContent.TileType<HardJungleChestTile>(), ChestPool.JungleHard);
        HardmodeConvert[ChestID.Marble] = new HardmodeInfo((TileKey)ModContent.TileType<HardMarbleChestTile>(), ChestPool.UndergroundHard);
        HardmodeConvert[ChestID.Mushroom] = new HardmodeInfo((TileKey)ModContent.TileType<HardMushroomChestTile>(), ChestPool.UndergroundHard);
        HardmodeConvert[ChestID.Webbed] = new HardmodeInfo(ChestID.Spider, ChestPool.UndergroundHard);
        HardmodeConvert[ChestID.Sandstone] = new HardmodeInfo((TileKey)ModContent.TileType<HardSandstoneChestTile>(), ChestPool.DesertHard);
    }
}
