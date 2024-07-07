using AequusRemake.Content.Configuration;
using AequusRemake.Content.CrossMod.CalamityModSupport;
using AequusRemake.Content.Tiles.PollutedOcean.PolymerSands;
using AequusRemake.Core.Components;
using AequusRemake.Core.Util.Helpers;
using System;
using Terraria.IO;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace AequusRemake.Content.Biomes.PollutedOcean.Generation;

public sealed class PollutedBiomeStep : AGenStep {
    public override string InsertAfter => "Create Ocean Caves";

    private static int x;
    private static int y;
    private static int Left;
    private static int Right;
    private static int LeftPadded;
    private static int RightPadded;
    private static int direction;
    private static PollutedOceanGenerationSideConfig generationSide;

    protected override double GenWeight => 200f;

    public static int BeachSize { get; set; } = 400;
    public static int SurfaceWallPadding { get; set; } = 10;
    public static int SurfaceWallPaddingNoise { get; set; } = 10;
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

    public static int X => x;
    public static int Y => y;
    public static int _Left => Left;
    public static int _Right => Right;
    public static int _LeftPadded => LeftPadded;
    public static int _RightPadded => RightPadded;
    public static int Direction => direction;
    public static PollutedOceanGenerationSideConfig GenerationSide => generationSide;

    internal static ushort _polymerSand;
    internal static ushort _polymerSandstone;
    internal static ushort _polymerSandstoneWall;

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
        CalculateDimensions();
        PunchHoles(progress);
        AddRock(progress);
        PunchOres(progress);
    }

    private static PollutedOceanGenerationSideConfig GetGenerationSide(UnifiedRandom random) {
        var generationSide = GameplayConfig.Instance.PollutedOceanSide;

        if (generationSide == PollutedOceanGenerationSideConfig.CompleteRandom) {
            return random.NextBool() ? PollutedOceanGenerationSideConfig.DungeonSide : PollutedOceanGenerationSideConfig.JungleSide;
        }

        if (generationSide != PollutedOceanGenerationSideConfig.Automatic) {
            return generationSide;
        }

        if (CalamityMod.Enabled) {
            return PollutedOceanGenerationSideConfig.JungleSide;
        }
        return PollutedOceanGenerationSideConfig.DungeonSide;
    }

    private static void SetGenerationParameters(PollutedOceanGenerationSideConfig side) {
        direction = Main.dungeonX * 2 < Main.maxTilesX ? -1 : 1;
        if (side == PollutedOceanGenerationSideConfig.JungleSide) {
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

    private void GenerateSandCaverns(GenerationProgress progress, int bottom, ref int endX, int biomeSize, int biomeSizeFlat) {
        SetSubMessage(progress, "Sand");

        int k = 0;
        int j = y;
        do {
            double p = (bottom - j) / (double)(bottom - y);
            int size = (int)(Math.Sin(p * MathHelper.PiOver2) * biomeSize) + biomeSizeFlat;
            if (k % 80 == 0) {
                AddProtectedStructure(endX, j, (int)(size * 1.5f), (int)(size * 1.5f), 20);
            }
            SetProgress(progress, 1f - p, 0f, 0.25f);
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

    private static void EmitSand(int x, int y, int size, int direction, out int notReplaceable) {
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

                if (j < (int)Main.worldSurface && !tile.HasTile && tile.WallType == WallID.None) {
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

                if (!ReplaceableWall[tile.WallType]) {
                    continue;
                }

                if (tile.HasTile) {
                    if (!SafeTile[tile.TileType] || Main.wallDungeon[tile.WallType]) {
                        notReplaceable++;
                        continue;
                    }
                    if (!ReplaceableTile[tile.TileType]) {
                        continue;
                    }
                    if (Main.tileFrameImportant[tile.TileType]) {
                        WorldGen.KillTile(i, j);
                    }
                }

                tile.HasTile = true;
                tile.IsActuated = false;
                tile.TileType = _polymerSand;
                tile.WallType = _polymerSandstoneWall;
                tile.LiquidAmount = 0;
                //if (!Main.tile[i, j + 1].IsFullySolid() || (Main.tile[i, j - 1].IsFullySolid() && (!Main.tile[i - 1, j].IsFullySolid() || !Main.tile[i - 1, j + 1].IsFullySolid() || !Main.tile[i + 1, j].IsFullySolid() || !Main.tile[i + 1, j + 1].IsFullySolid()))) {
                //    tile.TileType = _polymerSandstone;
                //}
                //else {
                //    tile.TileType = _polymerSand;
                //}
                //tile.WallType = (ushort)ModContent.WallType<PolymerSandstoneWallHostile>();
            }
            if (Random.NextBool(3)) {
                worldSurface++;
            }
        }
    }

    private static void CalculateDimensions() {
        Left = 0;
        Right = Main.maxTilesX;
        for (int i = x; i < Main.maxTilesX; i++) {
            if (!ScanDownForAnyPollutedOceanTiles(i)) {
                Right = i;
                break;
            }
        }
        for (int i = x; i >= 0; i--) {
            if (!ScanDownForAnyPollutedOceanTiles(i)) {
                Left = i;
                break;
            }
        }
        LeftPadded = Math.Max(Left, 5);
        RightPadded = Math.Min(Right, Main.maxTilesX - 5);

        static bool ScanDownForAnyPollutedOceanTiles(int i) {
            for (int j = (int)Main.worldSurface; j < Main.maxTilesY; j++) {
                Tile tile = Main.tile[i, j];
                ushort tileType = tile.TileType;
                if (tileType == _polymerSand || tileType == _polymerSandstone || tile.WallType == _polymerSandstoneWall) {
                    return true;
                }
            }

            return false;
        }
    }

    private void PunchHoles(GenerationProgress progress) {
        SetSubMessage(progress, "Caves");

        for (int i = Left; i < Right; i++) {
            for (int j = 0; j < Main.maxTilesY; j++) {
                SetProgress(progress, RectangleProgress(i, j, Left, Right), 0.25f, 0.33f);
                if (Main.tile[i, j].TileType == _polymerSand && Random.NextBool(Main.tile[i, j].HasTile ? 200 : 10)) {
                    PunchHole(i, j, Random.Next(7, 28), Random.Next(8, 18));
                }
            }
        }
        for (int i = Left; i < Right; i++) {
            for (int j = (int)Main.worldSurface; j < Main.maxTilesY; j++) {
                SetProgress(progress, RectangleProgress(i, j, Left, Right), 0.33f, 0.5f);
                if (Main.tile[i, j].TileType == _polymerSand && Random.NextBool(2000)) {
                    PunchWaterHole(i, j, Random.Next(80, 100));
                }
            }
        }
    }

    private static void PunchHole(int i, int j, int sizeX, int sizeY) {
        int startX = i - sizeX;
        int endX = i + sizeX;
        int progressX = 0;
        for (int x = startX; x < endX; x++) {
            int circleSizeY = (int)Math.Round(Math.Sin(progressX / (double)sizeX * Math.PI) * sizeY / 2.0);
            int endY = j + circleSizeY;
            progressX++;
            for (int y = j - circleSizeY; y < endY; y++) {
                if (WorldGen.InWorld(x, y) && ReplaceableWall[Main.tile[x, y].WallType] && SafeTile[Main.tile[x, y].TileType]) {
                    if (y < Main.worldSurface && Main.tile[x, y].HasTile) {
                        Main.tile[x, y].WallType = _polymerSandstoneWall;
                    }
                    else {
                        WorldGen.KillWall(x, y);
                    }

                    WorldGen.KillTile(x, y);
                }
            }
        }
    }

    private static void PunchWaterHole(int i, int j, int size) {
        int startX = i - size;
        int endX = i + size;
        int progressX = 0;
        for (int x = startX; x < endX; x++) {
            int sizeY = (int)Math.Round(Math.Sin(progressX / (double)size * Math.PI) * size / 2.0);
            int endY = j + sizeY;
            progressX++;
            for (int y = j - sizeY; y < endY; y++) {
                if (WorldGen.InWorld(x, y) && ReplaceableWall[Main.tile[x, y].WallType]) {
                    var tile = Main.tile[x, y];
                    tile.LiquidAmount = 255;
                    tile.LiquidType = LiquidID.Water;
                }
            }
        }
    }

    private void AddRock(GenerationProgress progress) {
        SetSubMessage(progress, "Detailing");

        int surface = (int)Main.worldSurface + SurfaceWallPadding + SurfaceWallPaddingNoise;

        for (int i = LeftPadded; i < RightPadded; i++) {
            for (int j = 5; j < surface; j++) {
                SetProgress(progress, RectangleProgress(i, j, LeftPadded, RightPadded, 0, surface), 0.5f, 0.6f);
                Tile tile = Main.tile[i, j];
                ushort tileType = tile.TileType;
                if (tileType == _polymerSand || tileType == _polymerSandstone || tile.WallType == _polymerSandstoneWall) {
                    FillSurfaceWalls(i, j);
                    break;
                }
            }
        }

        for (int i = LeftPadded; i < RightPadded; i++) {
            for (int j = 5; j < Main.maxTilesY - 5; j++) {
                SetProgress(progress, RectangleProgress(i, j, LeftPadded, RightPadded), 0.6f, 0.75f);
                Tile tile = Main.tile[i, j];
                ushort tileType = tile.TileType;
                if (tile.TileType == _polymerSand) {
                    if (tile.HasTile) {
                        tile.WallType = _polymerSandstoneWall;
                        if (!Main.tile[i, j + 1].IsFullySolid() || Main.tile[i, j - Random.Next(3, 6)].IsFullySolid()) {
                            tile.TileType = _polymerSandstone;
                        }
                    }
                    else {
                        tile.WallType = WallID.None;
                    }
                }
            }
        }

        for (int m = 0; m < 3; m++) {
            for (int i = Left; i < Right; i++) {
                for (int j = 0; j < Main.maxTilesY; j++) {
                    SetProgress(progress, m / 3f + RectangleProgress(i, j, Left, Right) * 0.33f, 0.75f, 0.85f);
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
                            if (j >= surface && Random.NextBool(180)) {
                                GrowWormyWall(i, j);
                            }
                        }
                        //else if (Main.tileSand[Main.tile[i, j].TileType]) {
                        //    if (!TileHelper.ScanTilesSquare(i, j, Random.Next(5, 12), TileHelper.IsNotSolid)) {
                        //        Main.tile[i, j].TileType = TileID.HardenedSand;
                        //    }
                        //}
                    }
                }
            }
        }
    }

    private static void FillSurfaceWalls(int i, int j) {
        int surface = (int)Main.worldSurface + SurfaceWallPadding + Random.Next(SurfaceWallPaddingNoise);
        for (; j < surface; j++) {
            if (Main.tile[i, j].WallType == WallID.None) {
                Main.tile[i, j].WallType = _polymerSandstoneWall;
            }
        }
    }

    private static void GrowWormyWall(int x, int y) {
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

    private void PunchOres(GenerationProgress progress) {
        SetSubMessage(progress, "Ores");

        int[] oreChoices = GetOreChoices();

        for (int i = Left; i < Right; i++) {
            for (int j = 0; j < Main.maxTilesY; j++) {
                SetProgress(progress, RectangleProgress(i, j, Left, Right), 0.85f, 1f);
                if (Main.tile[i, j].TileType == _polymerSandstone && Random.NextBool(600) && !TileHelper.ScanTilesSquare(i, j, 5, TileHelper.IsNotSolid,
                    (i, j) => Main.tile[i, j].HasTile && TileID.Sets.Ore[Main.tile[i, j].TileType])) {
                    PunchOre(i, j, Random.Next(oreChoices));
                }
            }
        }
    }

    private static void PunchOre(int i, int j, int oreTileId) {
        WorldGen.OreRunner(i, j, Random.Next(3, 7), Random.Next(8, 17), (ushort)oreTileId);
    }

    private static int[] GetOreChoices() {
        if (Main.drunkWorld) {
            return new int[8] {
                TileID.Copper,
                TileID.Iron,
                TileID.Silver,
                TileID.Gold,
                TileID.Tin,
                TileID.Lead,
                TileID.Tungsten,
                TileID.Platinum,
            };
        }

        int[] oreChoices = new int[4] {
            TileID.Copper,
            TileID.Iron,
            TileID.Silver,
            TileID.Gold,
        };

        if (GenVars.copper == TileID.Copper) {
            oreChoices[0] = TileID.Tin;
        }
        if (GenVars.iron == TileID.Iron) {
            oreChoices[1] = TileID.Lead;
        }
        if (GenVars.silver == TileID.Silver) {
            oreChoices[2] = TileID.Tungsten;
        }
        if (GenVars.gold == TileID.Gold) {
            oreChoices[3] = TileID.Gold;
        }

        return oreChoices;
    }

    private static void GenerateTileArrays() {
        int length = TileLoader.TileCount;
        ReplaceableTile = ArrayHelper.PopulateNewArray((i) => true, length);
        SafeTile = ArrayHelper.PopulateNewArray((i) => true, length);

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
            if (TileID.Sets.Grass[i] || TileID.Sets.Ore[i]) {
                ReplaceableTile[i] = true;
            }
        }

        // Manual assignment
        ReplaceableTile[TileID.Traps] = false;

        ReplaceableTile[TileID.LihzahrdBrick] = false;
        SafeTile[TileID.LihzahrdBrick] = false;

        ReplaceableTile[TileID.MushroomGrass] = true;

        ReplaceableTile[TileID.Marble] = true;
        ReplaceableTile[TileID.Granite] = true;
        ReplaceableTile[TileID.MarbleBlock] = true;
        ReplaceableTile[TileID.GraniteBlock] = true;
        ReplaceableTile[TileID.HardenedSand] = true;
        ReplaceableTile[_polymerSand] = true;
        SafeTile[_polymerSand] = true;
        ReplaceableTile[_polymerSandstone] = true;
        SafeTile[_polymerSandstone] = true;
    }

    private static void GenerateWallArrays() {
        int length = WallLoader.WallCount;
        ReplaceableWall = ArrayHelper.PopulateNewArray((i) => true, length);

        for (int i = 0; i < length; i++) {
            if (Main.wallDungeon[i] || WallID.Sets.Corrupt[i] || WallID.Sets.Crimson[i] || WallID.Sets.Hallow[i]) {
                ReplaceableWall[i] = false;
            }
        }

        // Manual assignment
        ReplaceableWall[WallID.None] = true;
        ReplaceableWall[WallID.LihzahrdBrickUnsafe] = false;
        ReplaceableWall[_polymerSandstoneWall] = true;
    }

    public override void PostSetupContent() {
        _polymerSand = (ushort)ModContent.TileType<PolymerSand>();
        _polymerSandstone = (ushort)ModContent.TileType<PolymerSandstone>();
        _polymerSandstoneWall = (ushort)ModContent.WallType<PolymerSandstoneWallHostile>();
        GenerateTileArrays();
        GenerateWallArrays();
    }

    public void GetIterationValues(out int left, out int right, out int top, out int bottom) {
        left = _LeftPadded;
        right = _RightPadded;
        top = Math.Max(Y - 25, 10);
        bottom = Main.UnderworldLayer + 20;
    }
}