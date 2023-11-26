using Aequus.Common.WorldGeneration;
using Aequus.Content.Biomes.PollutedOcean.Tiles;
using Aequus.Content.Configuration;
using Aequus.Content.CrossMod;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.PollutedOcean;

public sealed class PollutedOceanGenerator : AequusGenStep {
    public override string InsertAfter => "Create Ocean Caves";

    private static int x;
    private static int y;
    private static int direction;
    private static POGenerationSide generationSide;

    protected override double GenWeight => 100f;

    public static int BeachSize { get; set; } = 400;
    public static float CreviceHeightPercent { get; set; } = 0.33f;
    public static int MaxTilesXHalf => Main.maxTilesX / 2;

    /// <summary>
    /// Similar to <see cref="ReplaceableTile"/>, except if tile Ids in this entry are true, the Crab Crevice will actively try to steer away from generating around them.
    /// </summary>
    public static bool[] DangerousTile { get; private set; }

    /// <summary>
    /// Lookup for tile Ids which are safe to generate over.
    /// </summary>
    public static bool[] ReplaceableTile { get; private set; }

    public static int X { get => x; }
    public static int Y { get => y; }
    public static int Direction { get => direction; }
    public static POGenerationSide GenerationSide { get => generationSide; }

    public override void PostSetupContent() {
        ReplaceableTile = EnumerableHelper.CreateArray(true, TileLoader.TileCount);
        DangerousTile = EnumerableHelper.CreateArray(false, TileLoader.TileCount);

        ReplaceableTile[TileID.Traps] = false;

        ReplaceableTile[TileID.LihzahrdBrick] = false;
        DangerousTile[TileID.LihzahrdBrick] = false;

        for (int i = 0; i < ReplaceableTile.Length; i++) {
            if (Main.tileDungeon[i]) {
                ReplaceableTile[i] = false;
                DangerousTile[i] = true;
            }
            else if (!Main.tileSolid[i] || Main.tileFrameImportant[i] || TileID.Sets.Clouds[i] || !TileID.Sets.GeneralPlacementTiles[i] || !TileID.Sets.CanBeClearedDuringGeneration[i] || !TileID.Sets.CanBeClearedDuringOreRunner[i]) {
                ReplaceableTile[i] = false;
            }
            else if (TileID.Sets.Stone[i]) {
                if (TileID.Sets.Corrupt[i] || TileID.Sets.Crimson[i] || TileID.Sets.Hallow[i]) {
                    ReplaceableTile[i] = false;
                    DangerousTile[i] = true;
                }
            }
            if (TileID.Sets.Grass[i]) {
                ReplaceableTile[i] = true;
            }
        }
    }

    private static POGenerationSide GetGenerationSide(UnifiedRandom random) {
        var generationSide = GameplayConfig.Instance.PollutedOceanSide;

        if (generationSide == POGenerationSide.CompleteRandom) {
            return random.NextBool() ? POGenerationSide.DungeonSide : POGenerationSide.JungleSide;
        }

        if (generationSide != POGenerationSide.Automatic) {
            return generationSide;
        }

        if (CalamityMod.IsEnabled) {
            return POGenerationSide.JungleSide;
        }
        return POGenerationSide.DungeonSide;
    }

    private static void SetGenerationParameters(POGenerationSide side) {
        direction = Main.dungeonX * 2 < Main.maxTilesX ? -1 : 1;
        if (side == POGenerationSide.JungleSide) {
            direction = -direction;
        }

        x = MaxTilesXHalf + (MaxTilesXHalf - BeachSize) * direction;
        y = (int)Main.worldSurface;
        int airBlocks = 0;
        const int AirBlocksMax = 15;
        for (; y > Helper.ZoneSkyHeightY; y--) {
            if (Main.tile[x, y].IsSolid() || Main.tile[x, y].WallType > WallID.None) {
                airBlocks++;
                if (airBlocks > AirBlocksMax) {
                    y -= AirBlocksMax;
                    break;
                }
            }
            else {
                airBlocks = 0;
            }
        }
    }

    private static void EmitSand(int x, int y, int size, int direction, out int notReplaceable) {
        int startX = Math.Max(x - size, 1);
        int startY = Math.Max(y - size, 1);
        int endX = Math.Min(x + size, Main.maxTilesX - 1);
        int endY = Math.Min(y + size, Main.maxTilesY - 1);
        var sedimentaryRock = (ushort)ModContent.TileType<SedimentaryRock>();
        notReplaceable = 0;
        int circleSize = size * size / 2;
        int worldSurface = (int)Main.worldSurface / 2;

        if (direction == -1) {
            for (int i = startX; i < endX; i++) {
                EmitVerticalSandStrip(i, ref notReplaceable);
            }
        }
        else {
            for (int i = endX; i > startX; i--) {
                EmitVerticalSandStrip(i, ref notReplaceable);
            }
        }

        void EmitVerticalSandStrip(int i, ref int notReplaceable) {
            for (int j = startY; j < endY; j++) {
                var tile = Main.tile[i, j];
                if (!tile.HasTile) {
                    continue;
                }

                if (j < worldSurface) {
                    if (j <= worldSurface - 10 || !Random.NextBool(10)) {
                        continue;
                    }
                }

                int k = i - x;
                int l = j - y;
                var length = k * k + l * l;
                if (length >= circleSize) {
                    if (length >= circleSize + 5000 || !Random.NextBool(10)) {
                        continue;
                    }
                }

                if (DangerousTile[tile.TileType] || Main.wallDungeon[tile.WallType]) {
                    notReplaceable++;
                    continue;
                }
                if (!ReplaceableTile[tile.TileType]) {
                    continue;
                }

                tile.IsActuated = false;
                if (!Main.tile[i, j + 1].IsFullySolid() || (Main.tile[i, j - 1].IsFullySolid() && (!Main.tile[i - 1, j].IsFullySolid() || !Main.tile[i - 1, j + 1].IsFullySolid() || !Main.tile[i + 1, j].IsFullySolid() || !Main.tile[i + 1, j + 1].IsFullySolid()))) {
                    tile.TileType = sedimentaryRock;
                }
                else {
                    tile.TileType = TileID.Sand;
                }
            }
            if (Random.NextBool(3)) {
                worldSurface++;
            }
        }
    }

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        generationSide = GetGenerationSide(Random);
        SetGenerationParameters(generationSide);

        int height = (int)(Main.maxTilesY * CreviceHeightPercent);
        int bottom = Math.Min(y + height, Main.UnderworldLayer - 130);
        int endX = x;
        float size = Main.maxTilesX / (float)WorldGen.WorldSizeSmallX;
        int biomeSize = (int)(size * 200f);
        int biomeSizeFlat = (int)(size * 150f);

        SetMessage(progress, Random.Next(9));

        for (int j = y; j < bottom; j += 2) {
            double p = (bottom - j) / (double)(bottom - y);
            SetProgress(progress, 1f - p, 0f, Weight);
            EmitSand(endX, j, (int)(Math.Sin(p * MathHelper.PiOver2) * biomeSize) + biomeSizeFlat, direction, out int notReplaceable);
            if (notReplaceable > 150) {
                endX += direction * Random.Next(10);
            }
            else if (Random.NextBool(12)) {
                endX += direction * Random.Next(-11, 2);
            }
        }
    }
}