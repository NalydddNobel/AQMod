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

    protected override double GenWeight => 200f;

    public static int BeachSize { get; set; } = 400;
    public static float CreviceHeightPercent { get; set; } = 0.33f;
    public static int MaxTilesXHalf => Main.maxTilesX / 2;

    /// <summary>
    /// Similar to <see cref="ReplaceableTile"/>, except if tile Ids in this entry are false, the Crab Crevice will actively try to steer away from generating around them.
    /// </summary>
    public static bool[] SafeTile { get; private set; }

    /// <summary>
    /// Lookup for tile Ids which are safe to generate over.
    /// </summary>
    public static bool[] ReplaceableTile { get; private set; }
    public static bool[] ReplaceableWall { get; private set; }

    public static int X { get => x; }
    public static int Y { get => y; }
    public static int Direction { get => direction; }
    public static POGenerationSide GenerationSide { get => generationSide; }

    private ushort _polymerSandstone;
    private ushort _polymerSandstoneWall;

    private void GenerateTileArrays() {
        int length = TileLoader.TileCount;
        ReplaceableTile = EnumerableHelper.CreateArray(true, length);
        SafeTile = EnumerableHelper.CreateArray(true, length);

        for (int i = 0; i < length; i++) {
            if (Main.tileDungeon[i]) {
                ReplaceableTile[i] = false;
                SafeTile[i] = false;
            }
            else if (!Main.tileSolid[i] || Main.tileFrameImportant[i] || TileID.Sets.Clouds[i] || !TileID.Sets.GeneralPlacementTiles[i] || !TileID.Sets.CanBeClearedDuringGeneration[i] || !TileID.Sets.CanBeClearedDuringOreRunner[i]) {
                ReplaceableTile[i] = false;
            }
            else if (TileID.Sets.Stone[i]) {
                if (TileID.Sets.Corrupt[i] || TileID.Sets.Crimson[i] || TileID.Sets.Hallow[i]) {
                    ReplaceableTile[i] = false;
                    SafeTile[i] = false;
                }
            }
            if (TileID.Sets.Grass[i]) {
                ReplaceableTile[i] = true;
            }
        }

        // Manual assignment
        ReplaceableTile[TileID.Traps] = false;

        ReplaceableTile[TileID.LihzahrdBrick] = false;
        SafeTile[TileID.LihzahrdBrick] = false;

        ReplaceableTile[TileID.MushroomGrass] = true;

        ReplaceableTile[_polymerSandstone] = true;
        SafeTile[_polymerSandstone] = true;
    }

    private void GenerateWallArrays() {
        int length = WallLoader.WallCount;
        ReplaceableWall = EnumerableHelper.CreateArray(true, length);

        for (int i = 0; i < length; i++) {
            if (Main.wallDungeon[i]) {
                ReplaceableWall[i] = false;
            }
        }

        // Manual assignment
        ReplaceableWall[WallID.LihzahrdBrickUnsafe] = false;
        ReplaceableWall[_polymerSandstoneWall] = false;
    }

    public override void PostSetupContent() {
        _polymerSandstone = (ushort)ModContent.TileType<PolymerSandstone>();
        _polymerSandstoneWall = (ushort)ModContent.WallType<PolymerSandstoneWallHostile>();
        GenerateTileArrays();
        GenerateWallArrays();
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

    private void EmitSand(int x, int y, int size, int direction, out int notReplaceable) {
        int startX = Math.Max(x - size, 1);
        int startY = Math.Max(y - size, 1);
        int endX = Math.Min(x + size, Main.maxTilesX - 1);
        int endY = Math.Min(y + size, Main.maxTilesY - 1);
        notReplaceable = 0;
        int circleSize = size * size / 2;
        int worldSurface = (int)Main.worldSurface / 3 * 2;

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

                if (!tile.HasTile && tile.WallType == WallID.None) {
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

                if (ReplaceableWall[tile.WallType]) {
                    tile.WallType = WallID.None;
                }

                if (!tile.HasTile) {
                    continue;
                }

                if (!SafeTile[tile.TileType] || Main.wallDungeon[tile.WallType]) {
                    notReplaceable++;
                    continue;
                }
                if (!ReplaceableTile[tile.TileType]) {
                    continue;
                }

                tile.IsActuated = false;
                if (!Main.tile[i, j + 1].IsFullySolid() || (Main.tile[i, j - 1].IsFullySolid() && (!Main.tile[i - 1, j].IsFullySolid() || !Main.tile[i - 1, j + 1].IsFullySolid() || !Main.tile[i + 1, j].IsFullySolid() || !Main.tile[i + 1, j + 1].IsFullySolid()))) {
                    tile.TileType = _polymerSandstone;
                }
                else {
                    tile.TileType = TileID.Sand;
                }
                tile.WallType = (ushort)ModContent.WallType<PolymerSandstoneWallHostile>();
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

        GenerateSandCaverns(progress, bottom, ref endX, biomeSize, biomeSizeFlat);
        AddExtraRock(progress);
    }

    private void GenerateSandCaverns(GenerationProgress progress, int bottom, ref int endX, int biomeSize, int biomeSizeFlat) {
        int k = 0;
        int j = y;
        do {
            double p = (bottom - j) / (double)(bottom - y);
            int size = (int)(Math.Sin(p * MathHelper.PiOver2) * biomeSize) + biomeSizeFlat;
            if (k % 80 == 0) {
                AddProtectedStructure(endX, j, (int)(size * 1.5f), (int)(size * 1.5f), 20);
            }
            SetProgress(progress, 1f - p, 0f, Weight / 2f);
            EmitSand(endX, j, size, direction, out int notReplaceable);
            if (notReplaceable > 150) {
                endX += direction * Random.Next(10);
            }
            else if (Random.NextBool(12)) {
                endX += direction * Random.Next(-11, 2);
            }

            k++;
            j += 2;
        }
        while (j < bottom);
    }

    private void AddExtraRock(GenerationProgress progress) {
        for (int m = 0; m < 3; m++) {
            for (int i = 0; i < Main.maxTilesX; i++) {
                for (int j = 0; j < Main.maxTilesY; j++) {
                    SetProgress(progress, m / 3f + (i * Main.maxTilesY + j) / (double)(Main.maxTilesX + Main.maxTilesY) * 0.33f, Weight / 2f, Weight);
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].WallType == _polymerSandstoneWall) {
                        if (Main.tile[i, j].TileType == _polymerSandstone) {
                            j++;
                            for (int k = i - 1; k <= i + 1; k++) {
                                for (int l = j - 1; l <= j + 1; l++) {
                                    if (WorldGen.InWorld(k, l) && Main.tileSand[Main.tile[k, l].TileType]) {
                                        if (Random.NextBool(5)) {
                                            Main.tile[k, l].TileType = _polymerSandstone;
                                        }
                                    }
                                }
                            }
                            if (WorldGen.InWorld(i, j) && Random.NextBool(40)) {
                                GrowWormyWall(i, j);
                            }
                        }
                        else if (Main.tileSand[Main.tile[i, j].TileType]) {
                            if (!TileHelper.ScanTilesSquare(i, j, Random.Next(5, 12), TileHelper.IsNotSolid)) {
                                Main.tile[i, j].TileType = TileID.HardenedSand;
                            }
                        }
                    }
                }
            }
        }
    }

    private void GrowWormyWall(int x, int y) {
        var velocity = new Vector2(Random.NextFloat(-1f, 1f), Random.NextFloat(-0.2f, 1f));

        var position = new Vector2(x, y);
        int size = WorldGen.genRand.Next(2, 5);
        for (int i = 0; i < 1000; i++) {
            position += velocity;
            if (velocity.Y < 0f) {
                velocity.Y *= 0.95f;
            }

            if (velocity.Length() < 1f) {
                velocity = Vector2.Normalize(velocity);
            }

            var tilePosition = position.ToPoint();
            if (!WorldGen.InWorld(tilePosition.X, tilePosition.Y, 10)) {
                if (i > 15) {
                    break;
                }

                continue;
            }
            size = Math.Clamp(size + WorldGen.genRand.Next(-1, 2), 1, 5);
            size *= 2;
            for (int k = -size; k <= size; k++) {
                for (int l = -size; l <= size; l++) {
                    if (new Vector2(k, l).Length() <= size / 2f) {
                        var place = tilePosition + new Point(k - size / 2, l - size / 2);
                        if (ReplaceableWall[Main.tile[place].WallType]) {
                            Main.tile[place].WallType = _polymerSandstoneWall;
                        }
                    }
                }
            }
            if (Main.tile[tilePosition].IsFullySolid()) {
                if (i > 15) {
                    break;
                }
            }
            size /= 2;
            velocity = velocity.RotatedBy(WorldGen.genRand.NextFloat(WorldGen.genRand.NextFloat(-0.3f, 0.01f), WorldGen.genRand.NextFloat(0.01f, 0.3f)));
        }
    }
}