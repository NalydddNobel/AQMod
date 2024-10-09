using Aequus.Content.Items.Consumable.PermaPowerups.BreathCrystal;
using Aequus.Systems.WorldGeneration;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Content.World.Generation;

public class AquaCrystalsStep : AGenStep {
    public override string InsertAfter => "Life Crystals";

    public HashSet<int> IncreasedSpawnWalls = [];

    public override void Load() {
#if POLLUTED_OCEAN
        IncreasedSpawnWalls.Add(ModContent.WallType<Tiles.PollutedOcean.PolymerSands.PolymerSandstoneWallHostile>());
        IncreasedSpawnWalls.Add(ModContent.WallType<Tiles.PollutedOcean.PolymerSands.PolymerSandstoneWall>());
#endif
#if !CRAB_CREVICE_DISABLE
        IncreasedSpawnWalls.Add(ModContent.WallType<global::Aequus.Tiles.CrabCrevice.SedimentaryRockWallPlaced>());
        IncreasedSpawnWalls.Add(ModContent.WallType<global::Aequus.Tiles.CrabCrevice.SedimentaryRockWallFriendlyPlaced>());
#endif
    }

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        SetMessage(progress);
        GenTools.Random(Main.maxTilesX * Main.maxTilesY / 40, TryPlace, minY: (int)Main.worldSurface, maxY: Main.UnderworldLayer);
    }

    void TryPlace(int X, int Y, UnifiedRandom rng) {
        Tile tile = Main.tile[X, Y];
        Tile below = Framing.GetTileSafely(X, Y + 1);

        // Reduced chance to spawn in front of invalid walls.
        if (!IncreasedSpawnWalls.Contains(tile.WallType) && !WorldGen.genRand.NextBool(4)) {
            return;
        }

        if (!tile.HasTile && tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water && below.IsSolid()) {
            WorldGen.PlaceTile(X, Y, ModContent.TileType<BreathCrystalTile>());
#if DEBUG
            if (tile.TileType == ModContent.TileType<BreathCrystalTile>()) {
                Mod.Logger.Debug("Successfully placed breath crystal!");
            }
#endif
        }
    }
}
