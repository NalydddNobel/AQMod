﻿#if !CRAB_CREVICE_DISABLE
using Aequus.Common.Tiles;
using Aequus.Common.World;
using Aequus.Common.World.GenShapes;
using Aequus.CrossMod;
using Aequus.CrossMod.ThoriumModSupport;
using Aequus.Items.Materials.PearlShards;
using Aequus.Tiles.CrabCrevice;
using Aequus.Tiles.CrabCrevice.Ambient;
using System;
using System.Collections.Generic;
using Terraria.WorldBuilding;

namespace Aequus.Content.Biomes.CrabCrevice;
public class CrabCreviceGenerator : Generator {
    public int size;
    public int nextChestLoot;
    public int wantedDirection;
    public Point location;
    public Point genLocation;

    public int LeftX(int sizeX) {
        return Math.Clamp(AequusWorld.Structures.GetLocation("CrabCrevice").GetValueOrDefault(new Point(0, 0)).X, 5, Main.maxTilesX - sizeX - 5);
    }

    public void Reset() {
        nextChestLoot = 0;
        location = new Point();
        size = Main.maxTilesX / 28;
        if (size > 200) {
            size -= 200;
            size /= 2;
            size += 200;
        }
    }

    public bool ProperCrabCreviceAnchor(int x, int y) {
        return !Main.tile[x, y].HasTile && Main.tile[x, y + 1].HasTile && (Main.tileSand[Main.tile[x, y + 1].TileType] || Main.tile[x, y + 1].TileType == TileID.ShellPile);
    }

    private bool CanOverwriteTile(Tile tile) {
        return !Main.tileDungeon[tile.TileType] && !Main.wallDungeon[tile.WallType] && tile.TileType != TileID.LihzahrdBrick;
    }

    public void CreateSandAreaForCrevice(int x, int y) {
        if (x - size < 10) {
            x = size + 10;
        }
        else if (x + size > Main.maxTilesX - 10) {
            x = Main.maxTilesX - 10 - size;
        }
        if (y - size < 10) {
            y = size + 10;
        }
        else if (y + size > Main.maxTilesY - 10) {
            y = Main.maxTilesY - 10 - size;
        }

        List<Point> placeTiles = new();
        for (int i = 0; i < size * 2; i++) {
            // A bit overkill of an extra check, but whatever
            for (int j = 0; j < size * 5; j++) {
                int x2 = x + i - size;
                int y2 = y + j - size;
                int x3 = x2 - x;
                int y3 = y2 - y;
                if (Math.Sqrt(x3 * x3 + y3 * y3 * 0.175f) <= size) {
                    if (CanOverwriteTile(Main.tile[x2, y2]) && (!Main.remixWorld || Rand.NextBool(10))) {
                        if (Main.tile[x2, y2].HasTile) {
                            placeTiles.Add(new Point(x2, y2));
                        }
                    }
                }
            }
        }

        for (int i = 0; i < placeTiles.Count; i++) {
            int x2 = placeTiles[i].X;
            int y2 = placeTiles[i].Y;
            if (y2 > (int)Main.worldSurface) {
                for (int m = -2; m <= 2; m++) {
                    for (int n = -2; n <= 2; n++) {
                        Main.tile[x2 + m, y2 + n].Active(value: true);
                        if (y2 < Main.worldSurface + 25) {
                            if (!WorldGen.genRand.NextBool(25 + ((int)Main.worldSurface - y2) + 2)) {
                                Main.tile[x2 + m, y2 + n].TileType = TileID.Sand;
                                continue;
                            }
                        }
                        Main.tile[x2 + m, y2 + n].TileType = TileID.HardenedSand;
                    }
                }
            }
            else {
                for (int m = -2; m <= 2; m++) {
                    for (int n = -2; n <= 2; n++) {
                        if (!Main.tile[x2 + m, y2 + n].HasTile && !Main.tile[x2 + m, y2 + n].SolidType() && Main.tile[x2 + m, y2 + n].LiquidAmount > 0) {
                            continue;
                        }
                        Main.tile[x2 + m, y2 + n].Active(value: true);
                        Main.tile[x2 + m, y2 + n].TileType = TileID.Sand;
                    }
                }
            }
        }
    }

    public bool HasUnOverwriteableTiles(LegacyCircle circle) {
        for (int i = 0; i < circle.Radius * 2; i++) {
            for (int j = 0; j < circle.Radius * 2; j++) {
                int x2 = circle.X + i - circle.Radius;
                int y2 = circle.Y + j - circle.Radius;
                if (circle.Inside(x2, y2) && !CanOverwriteTile(Main.tile[x2, y2])) {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsValidCircleForGeneratingCave(int x, int y, int radius) {
        return IsValidCircleForGeneratingCave(new LegacyCircle(x, y, radius));
    }

    private bool IsValidCircleForGeneratingCave(LegacyCircle circle) {
        const int wallSize = 5;
        for (int i = 0; i < circle.Radius * 2; i++) {
            for (int j = 0; j < circle.Radius * 2; j++) {
                int x = circle.X + i - circle.Radius;
                int y = circle.Y + j - circle.Radius;
                if (circle.Inside(x, y)) {
                    for (int k = -wallSize; k <= wallSize; k++) {
                        for (int l = -wallSize; l <= wallSize; l++) {
                            if ((!Main.tile[x + k, y + l].HasTile || !Main.tile[x + k, y + l].SolidType()) && CanOverwriteTile(Main.tile[x + k, y + l])) {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return true;
    }

    public bool GenerateCreviceCave(int x, int y, int minScale, int maxScale, int steps) {
        List<LegacyCircle> validCircles = new();
        for (int i = maxScale; i > minScale; i--) {
            var c = LegacyCircle.FixedCircle(x, y, i);
            if (IsValidCircleForGeneratingCave(c)) {
                validCircles.Add(c);
                break;
            }
        }
        if (validCircles.Count == 0) {
            return false;
        }
        validCircles.Add(validCircles[0]
            .GetRandomCircleInsideCircle(validCircles[0].Radius / 3, minScale, maxScale, WorldGen.genRand, IsValidCircleForGeneratingCave));
        if (validCircles[1].IsInvalid || HasUnOverwriteableTiles(validCircles[1])) {
            return false;
        }
        for (int i = 0; i < steps; i++) {
            int chosenCircle = WorldGen.genRand.Next(validCircles.Count);
            validCircles.Add(validCircles[chosenCircle]
                .GetRandomCircleInsideCircle(validCircles[chosenCircle].Radius / 4, minScale, maxScale, WorldGen.genRand, IsValidCircleForGeneratingCave));
            if (validCircles[^1].IsInvalid || HasUnOverwriteableTiles(validCircles[^1])) {
                //Main.NewText("c" + (i + 2) + " was considered invalid!");
                return false;
            }
        }

        for (int k = 0; k < validCircles.Count; k++) {
            for (int i = 0; i < validCircles[k].Radius * 2; i++) {
                for (int j = 0; j < validCircles[k].Radius * 2; j++) {
                    int x2 = validCircles[k].X + i - validCircles[k].Radius;
                    int y2 = validCircles[k].Y + j - validCircles[k].Radius;
                    if (validCircles[k].Inside(x2, y2)) {
                        for (int m = -2; m <= 2; m++) {
                            for (int n = -2; n <= 2; n++) {
                                Main.tile[x2 + m, y2 + n].Active(value: true);
                                Main.tile[x2 + m, y2 + n].TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                                if (y2 < Main.worldSurface)
                                    Main.tile[x2 + m, y2 + n].WallType = (ushort)ModContent.WallType<SedimentaryRockWallPlaced>();
                                else if (y2 < Main.worldSurface - 25 && !WorldGen.genRand.NextBool(25))
                                    Main.tile[x2 + m, y2 + n].WallType = (ushort)ModContent.WallType<SedimentaryRockWallPlaced>();
                            }
                        }
                    }
                }
            }
        }

        byte minWater = Math.Min((byte)(255 - validCircles[0].Radius / 24 + validCircles[0].Y / 10), (byte)253);
        byte maxWater = 255;
        if (WorldGen.genRand.NextBool(4)) {
            minWater /= 6;
            maxWater = (byte)(minWater + 2);
        }
        else if (WorldGen.genRand.NextBool()) {
            minWater *= 4;
            if (minWater > 253) {
                minWater = 253;
            }
            maxWater = 255;
        }
        else if (minWater < 150) {
            maxWater = 150;
        }

        for (int k = 0; k < validCircles.Count; k++) {
            for (int i = 0; i < validCircles[k].Radius * 2; i++) {
                for (int j = 0; j < validCircles[k].Radius * 2; j++) {
                    int x2 = validCircles[k].X + i - validCircles[k].Radius;
                    int y2 = validCircles[k].Y + j - validCircles[k].Radius;
                    if (validCircles[k].Inside(x2, y2)) {
                        var tile = Main.tile[x2, y2];
                        tile.HasTile = false;
                        tile.LiquidType = Main.notTheBeesWorld ? LiquidID.Honey : LiquidID.Water;
                        if (minWater > 100 && Main.tile[x2, y2 + 1].HasTile && Main.tile[x2, y2 + 1].SolidType()) {
                            tile.LiquidAmount = 255;
                        }
                        else {
                            tile.LiquidAmount = (byte)WorldGen.genRand.Next(minWater, maxWater);
                        }
                    }
                }
            }
        }

        var caverPoint = WorldGen.genRand.Next(validCircles);
        if (caverPoint.Y > location.Y + 80 && WorldGen.genRand.NextBool(3)) {
            WorldGen.Caverer(caverPoint.X, caverPoint.Y);
        }
        return true;
    }

    public void GrowWalls(int x, int y) {
        if (x - size < 10) {
            x = size + 10;
        }
        else if (x + size > Main.maxTilesX - 10) {
            x = Main.maxTilesX - 10 - size;
        }
        if (y - size < 10) {
            y = size + 10;
        }
        else if (y + size > Main.maxTilesY - 10) {
            y = Main.maxTilesY - 10 - size;
        }
        for (int i = 0; i < size * 2; i++) {
            for (int j = 0; j < size * 6; j++) {
                int x2 = x + i - size;
                int y2 = y + j - size;
                int x3 = x2 - x;
                int y3 = y2 - y;
                if (Math.Sqrt(x3 * x3 * 0.8f + y3 * y3 * 0.125f) <= size) {
                    if (CanOverwriteTile(Main.tile[x2, y2])) {
                        if (Main.tile[x2, y2].HasTile && y2 > (int)Main.worldSurface && WorldGen.genRand.NextBool(16)) {
                            if (WorldGen.InWorld(x2, y2, 5)) {
                                bool allowedToCreatePillar = false;
                                for (int k = -1; k <= 1; k++) {
                                    for (int l = -1; l <= 1; l++) {
                                        if (!Main.tile[x2 + k, y2 + l].HasTile) {
                                            allowedToCreatePillar = true;
                                        }
                                    }
                                }
                                if (allowedToCreatePillar) {
                                    GrowWormyWall(x2, y2);
                                    SetProgress((i * size * 6 + j) / (float)((size * 2) * (size * 6)) * 0.19f + 0.01f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void GrowWormyWall(int x, int y) {
        var velo = new Vector2(WorldGen.genRand.NextFloat(-1f, 1f), WorldGen.genRand.NextFloat(-0.2f, 1f));
        var sedimentaryRockWall = (ushort)ModContent.WallType<SedimentaryRockWallPlaced>();
        var loc = new Vector2(x, y);
        int size = WorldGen.genRand.Next(2, 5);
        for (int i = 0; i < 1000; i++) {
            loc += velo;
            if (velo.Y < 0f)
                velo.Y *= 0.95f;
            if (velo.Length() < 1f)
                velo = Vector2.Normalize(velo);
            var p = loc.ToPoint();
            if (!WorldGen.InWorld(p.X, p.Y, 10)) {
                if (i > 15)
                    break;
                continue;
            }
            size = Math.Clamp(size + WorldGen.genRand.Next(-1, 2), 1, 5);
            if (!Main.remixWorld) {
                size *= 2;
            }
            for (int k = -size; k <= size; k++) {
                for (int l = -size; l <= size; l++) {
                    if (new Vector2(k, l).Length() <= size / 2f) {
                        var tile = Main.tile[p + new Point(k - size / 2, l - size / 2)];
                        tile.WallType = sedimentaryRockWall;
                    }
                }
            }
            if (Main.tile[p].IsFullySolid()) {
                if (i > 15)
                    break;
            }
            size /= 2;
            velo = velo.RotatedBy(WorldGen.genRand.NextFloat(WorldGen.genRand.NextFloat(-0.3f, 0.01f), WorldGen.genRand.NextFloat(0.01f, 0.3f)));
        }
    }

    public void PlaceChests(int leftX, int sizeX) {
        Reset();
        int amount = Main.maxTilesX * Main.maxTilesY / 128;
        if (Main.remixWorld) {
            amount *= 3;
        }
        for (int i = 0; i < amount; i++) {
            int randX = leftX + WorldGen.genRand.Next(sizeX);
            int randY = WorldGen.genRand.Next(10, Main.maxTilesY - 10);
            if (Main.tile[randX, randY].TileType == ModContent.TileType<SedimentaryRockTile>()) {
                randY--;
                int chestID = WorldGen.PlaceChest(randX, randY, type: TileID.Containers2, notNearOtherChests: true, style: ChestType.Reef);
                if (chestID != -1) {
                    var c = Main.chest[chestID];
                    FillChest(c.item, randY);
                }
            }
        }
    }

    public void FillChest(Item[] arr, int y) {
        int index = 0;
        var rand = WorldGen.genRand;

        var loot = CrabCreviceBiome.ChestPrimaryLoot[nextChestLoot % CrabCreviceBiome.ChestPrimaryLoot.Length];
        arr[index].SetDefaults(loot.item);
        arr[index++].stack = loot.RollStack(rand);

        nextChestLoot++;

        if (rand.NextBool()) {
            loot = rand.Next(CrabCreviceBiome.ChestSecondaryLoot);
            arr[index].SetDefaults(loot.item);
            arr[index++].stack = loot.RollStack(rand);
        }

        if (rand.NextBool()) {
            loot = rand.Next(CrabCreviceBiome.ChestSecondaryLoot);
            arr[index].SetDefaults(loot.item);
            arr[index++].stack = loot.RollStack(rand);
        }

        switch (rand.Next(3)) {
            case 0:
                arr[index++].SetDefaults(ItemID.CanOfWorms);
                break;
            case 1:
                arr[index].SetDefaults(ItemID.ApprenticeBait);
                arr[index++].stack = rand.Next(5) + 2;
                break;
            case 2:
                if (y > Main.worldSurface) {
                    arr[index].SetDefaults(ItemID.JourneymanBait);
                    arr[index++].stack = rand.Next(3) + 1;
                }
                break;
        }

        switch (rand.Next(8)) {
            case 0:
                arr[index].SetDefaults(ItemID.HealingPotion);
                arr[index++].stack = rand.Next(5) + 1;
                break;
            case 1:
                arr[index].SetDefaults(ItemID.RecallPotion);
                arr[index++].stack = rand.Next(5) + 1;
                break;
            case 2:
                arr[index].SetDefaults(ItemID.GillsPotion);
                arr[index++].stack = rand.Next(3) + 1;
                break;
            case 3:
                if (y > Main.worldSurface) {
                    arr[index].SetDefaults(ItemID.MiningPotion);
                    arr[index++].stack = rand.Next(3) + 1;
                }
                break;
            case 4:
                arr[index].SetDefaults(ItemID.BuilderPotion);
                arr[index++].stack = rand.Next(3) + 1;
                break;
            case 5:
                arr[index].SetDefaults(ItemID.HunterPotion);
                arr[index++].stack = rand.Next(3) + 1;
                break;
            case 6:
                if (y > Main.worldSurface) {
                    arr[index].SetDefaults(ItemID.TrapsightPotion);
                    arr[index++].stack = rand.Next(3) + 1;
                }
                break;
            case 7:
                arr[index].SetDefaults(ItemID.WaterWalkingPotion);
                arr[index++].stack = rand.Next(3) + 1;
                break;
        }

        switch (rand.Next(3)) {
            case 0:
                arr[index].SetDefaults(ItemID.CoralTorch);
                arr[index++].stack = rand.Next(8, 30);
                break;
            case 1:
                arr[index].SetDefaults(ItemID.Glowstick);
                arr[index++].stack = rand.Next(15, 101);
                break;
            case 2:
                arr[index].SetDefaults(ItemID.BottledWater);
                arr[index++].stack = rand.Next(8, 34);
                break;
        }

        arr[index].SetDefaults(ItemID.SilverCoin);
        arr[index++].stack = rand.Next(20, 91);
    }

    public void FixSand() {
        int sizeX = size * 2;
        var leftX = LeftX(sizeX);
        FixSand(leftX, sizeX);
    }
    public void FixSand(int leftX, int sizeX) {
        for (int i = leftX; i < leftX + sizeX; i++) {
            for (int j = 5; j < Main.maxTilesY - 5; j++) {
                var tile = Main.tile[i, j];
                if (tile.HasTile && (tile.TileType == TileID.Sand || tile.TileType == TileID.HardenedSand) && !Main.tile[i, j + 1].IsSolid()) {
                    tile.TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                    Main.tile[i, j - 1].TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                }
                if (tile.WallType == ModContent.WallType<SedimentaryRockWallPlaced>()) {
                    tile.LiquidType = Main.notTheBeesWorld ? LiquidID.Honey : LiquidID.Water;
                    tile.LiquidAmount = (byte)Math.Max(tile.LiquidAmount, WorldGen.genRand.Next(byte.MaxValue) + 1);
                }
            }
        }
    }

    public void DigTunnel(int x, int y) {
        int dir = x * 2 > Main.maxTilesX ? -1 : 1;
        var v = WorldGen.digTunnel(x, y, 0f, 1.25f, 80, WorldGen.genRand.Next(4, 7), Wet: true);
        for (int i = 0; i < 10; i++)
            v = WorldGen.digTunnel((int)v.X, (int)v.Y, dir * 2f, -1.33f, 4, WorldGen.genRand.Next(5, 9), Wet: false);
        for (int k = 0; k < 7; k++)
            v = WorldGen.digTunnel((int)v.X, (int)v.Y, dir * 0.3f, WorldGen.genRand.Next(1, 3), 10, WorldGen.genRand.Next(3, 5), Wet: true);
        for (int k = 0; k < size / 7; k++)
            v = WorldGen.digTunnel((int)v.X, (int)v.Y, WorldGen.genRand.Next(-2, 3), WorldGen.genRand.Next(1, 3), 10, WorldGen.genRand.Next(3, 5), Wet: true);
    }

    public void GrowPlants() {
        Reset();
        int sizeX = size * 2;
        var leftX = LeftX(sizeX);

        int max = Main.maxTilesX * Main.maxTilesY / 12;
        for (int i = 0; i < max; i++) {
            int randX = leftX + WorldGen.genRand.Next(sizeX);
            int randY = WorldGen.genRand.Next(10, Main.maxTilesY - 10);
            SetProgress(0.3f + (i / (float)max) * 0.7f);
            if (WorldGen.genRand.NextBool(50)) {
                PearlsTile.TryGrow(randX, randY);
            }
            SeaPickleTile.TryGrow(randX, randY);
            GrowFloorTiles(randX, randY);
            CrabHydrosailia.TryGrow(randX, randY);
        }
        for (int i = 0; i < max * 2; i++) {
            int randX = leftX + WorldGen.genRand.Next(sizeX);
            int randY = WorldGen.genRand.Next(10, Main.maxTilesY - 10);

            if (Main.tile[randX, randY].HasTile && Main.tile[randX, randY].TileType == ModContent.TileType<PearlsTileWhite>()) {
                Main.tile[randX, randY].TileType = (ushort)ModContent.TileType<PearlsTileHypnotic>();
                break;
            }
        }
    }

    public void FinalizeGeneration() {
        SetProgress(0f);
        DigTunnel(genLocation.X + wantedDirection * -33, genLocation.Y - 90);
        SetProgress(0.01f);
        GrowWalls(genLocation.X, genLocation.Y - 30);

        int sizeX = size * 2;
        var leftX = LeftX(sizeX);
        SetProgress(0.2f);
        PlaceChests(leftX, sizeX);
        SetProgress(0.25f);
        FixSand(leftX, sizeX);
        SetProgress(0.3f);
        GrowPlants();
        SetProgress(1f);
    }

    public void TransformPots() {
        Reset();
        int sizeX = size * 2;
        var leftX = LeftX(sizeX);
        int sedimentaryRockWall = ModContent.WallType<SedimentaryRockWallPlaced>();
        for (int i = leftX; i < leftX + sizeX; i++) {
            for (int j = 0; j < Main.UnderworldLayer; j++) {
                var tile = Main.tile[i, j];
                if (tile.HasTile && tile.TileType == TileID.Pots) {
                    for (int k = 0; k < 2; k++) {
                        for (int l = 0; l < 2; l++) {
                            var potTile = Main.tile[i + k, j + l];
                            if (potTile.TileType == TileID.Pots) {
                                potTile.TileType = (ushort)ModContent.TileType<CrabCrevicePot>();
                                potTile.TileFrameX %= 72;
                                potTile.TileFrameY %= 108;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="location"></param>
    /// <param name="genLocation"></param>
    private void SetLocationParams(ref int x, ref int y, ref Point location, ref Point genLocation) {
        // Sink the Crab Crevice if the Calamity+Thorium curse is active.
        if (ThoriumMod.Instance != null && CalamityMod.Instance != null) {
            int amount = Main.maxTilesY / 4;
            y += amount;
            location.Y += amount;
            genLocation.Y += amount;
        }
    }

    private void GenerateBigCave(int x, int y) {
        int finalCaveStart = -50;
        int finalCaveX;
        if (x < Main.maxTilesX / 2) {
            finalCaveX = x + WorldGen.genRand.Next(60);
        }
        else {
            finalCaveX = x + WorldGen.genRand.Next(-60, 0);
        }
        if (finalCaveX + finalCaveStart < 30) {
            finalCaveStart = 30 - finalCaveX;
        }
        int finalCaveEnd = 50;
        if (finalCaveX + finalCaveEnd > Main.maxTilesX - 30) {
            finalCaveEnd = Main.maxTilesX - 30 - finalCaveX;
        }
        List<LegacyCircle> finalCaveCircles = new();
        for (int k = finalCaveStart; k < finalCaveEnd; k++) {
            float finalCaveProgress = 1f / (finalCaveStart.Abs() + finalCaveEnd.Abs()) * k.Abs();
            LegacyCircle circle = new(finalCaveX + k, y + 180, WorldGen.genRand.Next(2, 14) + ((int)(Math.Sin((finalCaveProgress.Abs() - 0.5f) * MathHelper.Pi) * 9.0)).Abs());
            if (!HasUnOverwriteableTiles(circle)) {
                finalCaveCircles.Add(circle);
            }
        }

        var caverPoint = WorldGen.genRand.Next(finalCaveCircles);
        WorldGen.Caverer(caverPoint.X, caverPoint.Y);
        SetProgress(0.25f);

        for (int k = 0; k < finalCaveCircles.Count; k++) {
            for (int i = 0; i < finalCaveCircles[k].Radius * 2; i++) {
                for (int j = 0; j < finalCaveCircles[k].Radius * 2; j++) {
                    int x2 = finalCaveCircles[k].X + i - finalCaveCircles[k].Radius;
                    int y2 = finalCaveCircles[k].Y + j - finalCaveCircles[k].Radius;
                    if (finalCaveCircles[k].Inside(x2, y2)) {
                        for (int m = -2; m <= 2; m++) {
                            for (int n = -2; n <= 2; n++) {
                                Main.tile[x2 + m, y2 + n].Active(value: true);
                                Main.tile[x2 + m, y2 + n].TileType = (ushort)ModContent.TileType<SedimentaryRockTile>();
                            }
                        }
                    }
                }
            }
        }

        for (int k = 0; k < finalCaveCircles.Count; k++) {
            for (int i = 0; i < finalCaveCircles[k].Radius * 2; i++) {
                for (int j = 0; j < finalCaveCircles[k].Radius * 2; j++) {
                    int x2 = finalCaveCircles[k].X + i - finalCaveCircles[k].Radius;
                    int y2 = finalCaveCircles[k].Y + j - finalCaveCircles[k].Radius;
                    if (finalCaveCircles[k].Inside(x2, y2)) {
                        Main.tile[x2, y2].Active(value: false);
                        if (Main.tile[x2, y2 + 1].HasTile && Main.tile[x2, y2 + 1].SolidType()) {
                            Main.tile[x2, y2].LiquidAmount = 255;
                        }
                        else {
                            Main.tile[x2, y2].LiquidAmount = (byte)WorldGen.genRand.Next(10, 200);
                        }
                    }
                }
            }
        }
    }

    private void GenerateRemixCrevice(Rectangle ugDesert) {
        var center = ugDesert.Center;
        genLocation = center;
        GenerateBigCave(center.X, center.Y);

        var sedimentaryRockId = (ushort)ModContent.TileType<SedimentaryRockTile>();
        int endX = Math.Min(ugDesert.X + ugDesert.Width, Main.maxTilesX);
        int endY = Math.Min(ugDesert.Y + ugDesert.Height, Main.maxTilesY);
        for (int i = Math.Max(ugDesert.X, 0); i < endX; i++) {
            for (int j = Math.Max(ugDesert.Y, 0); j < endY; j++) {
                var tile = Main.tile[i, j];
                if (!tile.HasTile) {
                    if (tile.WallType == WallID.Sandstone) {
                        tile.LiquidType = Main.notTheBeesWorld ? LiquidID.Honey : LiquidID.Water;
                        tile.LiquidAmount = (byte)Rand.Next(byte.MinValue, byte.MaxValue);
                    }
                    continue;
                }
                if (tile.TileType == TileID.Sandstone && Rand.NextBool(3)) {
                    tile.TileType = sedimentaryRockId;
                }
                PearlsTile.TryGrow(i, j);
                SeaPickleTile.TryGrow(i, j);
                GrowFloorTiles(i, j);
                CrabHydrosailia.TryGrow(i, j);
            }
        }
    }

    private void GenerateCrabCrevice() {
        int x = location.X;
        int y = location.Y + 120;
        genLocation = new Point(x, y);

        SetLocationParams(ref x, ref y, ref location, ref genLocation);

        CreateSandAreaForCrevice(x + wantedDirection * -20, y + 40);
        SetProgress(0.1f);

        GenerateBigCave(x, y);

        for (int k = 0; k < size * 128; k++) {
            int caveX = x + (int)WorldGen.genRand.NextFloat(-size * 0.85f, size * 0.85f);
            int caveY = y + WorldGen.genRand.Next(55, (int)(size * 1.7f));
            int minScale = WorldGen.genRand.Next(4, 8);
            if (WorldGen.InWorld(x, y, 30) && GenerateCreviceCave(caveX, caveY, minScale, minScale + WorldGen.genRand.Next(4, 18), WorldGen.genRand.Next(80, 250))) {
                k += 375;
            }
            SetProgress(0.25f + k / (float)(size * 128f) * 0.75f);
        }
    }

    protected override void Generate() {
        Reset();
        SetProgress(0f);

        wantedDirection = 0;
        if (CalamityMod.Instance != null) {
            wantedDirection = Main.dungeonX * 2 < Main.maxTilesX ? 1 : -1;
        }
        else if (ThoriumMod.Instance != null) {
            wantedDirection = Main.dungeonX * 2 < Main.maxTilesX ? -1 : 1;
        }

        if (Main.remixWorld) {
            location.X = GenVars.UndergroundDesertLocation.X + GenVars.UndergroundDesertLocation.Width / 2;
            location.Y = GenVars.UndergroundDesertLocation.Y;
            GenerateRemixCrevice(GenVars.UndergroundDesertLocation);
        }
        else {
            for (int i = 0; i < 5000; i++) {
                int checkX = WorldGen.genRand.Next(90, 200);
                if (WorldGen.genRand.NextBool())
                    checkX = Main.maxTilesX - checkX;
                for (int checkY = 200; checkY < Main.worldSurface; checkY++) {
                    if (ProperCrabCreviceAnchor(checkX, checkY)) {
                        if (wantedDirection == 0 || location.X == 0) {
                            location.X = checkX;
                            location.Y = checkY;
                        }
                        else if (wantedDirection == -1) {
                            if (checkX * 2 < Main.maxTilesX) {
                                location.X = checkX;
                                location.Y = checkY;
                            }
                        }
                        else {
                            if (checkX * 2 > Main.maxTilesX) {
                                location.X = checkX;
                                location.Y = checkY;
                            }
                        }
                        i += 1000;
                        break;
                    }
                }
            }

            GenerateCrabCrevice();
        }

        SetProgress(1f);
        GenVars.structures.AddProtectedStructure(new Rectangle(location.X - size, location.Y - size, size * 2, size * 2).Fluffize(5));
        AequusWorld.Structures.Add("CrabCrevice", location);
    }

    public static bool IsWater(Tile tile) {
        return tile.LiquidAmount > 0 && (tile.LiquidType == LiquidID.Water || (Main.notTheBeesWorld && tile.LiquidType == LiquidID.Honey));
    }

    public static void GrowFloorTiles(int i, int j) {
        Tile aboveTile = Main.tile[i, j - 1];
        if (aboveTile.HasTile) {
            return;
        }

        bool isWater = IsWater(Main.tile[i, j - 1]);
        if (isWater && aboveTile.LiquidAmount == 255 && WorldGen.genRand.NextBool(8)) {
            WorldGen.PlaceTile(i, j, ModContent.TileType<CrabFloorPlants>(), mute: true, style: WorldGen.genRand.Next(16));
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, i, j);
            }
        }
        else if (isWater && aboveTile.LiquidAmount > 128 && WorldGen.genRand.NextBool(8)) {
            if (Main.rand.NextBool()) {
                WorldGen.PlaceTile(i, j - 1, TileID.BeachPiles, mute: true, style: WorldGen.genRand.Next(15));
            }
            else {
                WorldGen.PlaceTile(i, j - 1, TileID.Coral, mute: true);
            }
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, i, j - 1);
            }
        }
        else if (WorldGen.genRand.NextBool(8)) {
            if (Main.tile[i - 1, j - 1].HasTile || Main.tile[i + 1, j - 1].HasTile)
                return;
            WorldGen.PlaceTile(i, j - 1, ModContent.TileType<CrabGrassBig>(), mute: true);
            if (Main.netMode != NetmodeID.SinglePlayer) {
                NetMessage.SendTileSquare(-1, i, j - 1);
            }
        }
    }
}
#endif