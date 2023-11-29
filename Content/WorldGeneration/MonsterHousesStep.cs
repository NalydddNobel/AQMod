using Aequus.Common.WorldGeneration;
using Aequus.Content.Tiles.MonsterChest;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Aequus.Content.WorldGeneration;

public class MonsterHousesStep : AequusGenStep {
    public override string InsertAfter => "Buried Chests";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        int houseCount = (int)(Main.maxTilesX * Main.maxTilesY * 0.000016f);
        int bottom = Main.UnderworldLayer - 100;
        int amountGenerated = 0;
        for (int i = 0; i < houseCount; i++) {
            for (int attempts = 0; attempts < 10000; attempts++) {
                int randomX = Random.Next(50, Main.maxTilesX - 50);
                int randomY = Random.Next((int)Main.worldSurface + 50, bottom);

                if (Main.tile[randomX, randomY].WallType > WallID.None || !Main.tile[randomX, randomY].HasUnactuatedTile || TileHelper.ScanTiles(new(randomX - 40, randomY - 40, 80, 40), TileHelper.HasShimmer)) {
                    continue;
                }

                randomY--;
                if (WorldGen.AddBuriedChest(randomX, randomY)) {
                    if (TileID.Sets.IsAContainer[Main.tile[randomX, randomY].TileType]) {
                        var tileObjectData = TileObjectData.GetTileData(Main.tile[randomX, randomY]);
                        if (tileObjectData.Width == 2 && tileObjectData.Height == 2) {
                            var corner = TileHelper.GetCorner(randomX, randomY);
                            for (int k = 0; k < 2; k++) {
                                for (int l = 0; l < 2; l++) {
                                    var chestTile = Main.tile[corner.X + k, corner.Y + l];
                                    chestTile.TileType = (ushort)ModContent.TileType<MonsterChest>();
                                    chestTile.TileFrameX = (short)(18 * k);
                                    chestTile.TileFrameY = (short)(18 * l);
                                }
                            }
                        }
                    }
                    if (randomY < bottom) {
                        MineHouse(randomX, randomY);
                        amountGenerated++;
                        break;
                    }
                }
            }
        }
    }

    public bool MineHouse(int i, int j) {
        if (!WorldGen.InWorld(i, j, 50)) {
            return false;
        }

        int roofOffset = Random.Next(6, 12);
        int floorOffset = Random.Next(3, 6);
        int leftOffset = Random.Next(15, 30);
        int rightOffset = Random.Next(15, 30);

        if (WorldGen.SolidTile(i, j) || Main.tile[i, j].WallType > WallID.None) {
            return false;
        }

        int roof = j - roofOffset;
        int floor = j + floorOffset;
        ushort woodBlock = TileID.WoodBlock;
        ushort woodenBeam = TileID.WoodenBeam;
        ushort plankedWall = WallID.Planked;

        PlaceRoofAndFloor(i, j, roofOffset, floorOffset, leftOffset, rightOffset, roof, floor, woodBlock, plankedWall);

        int left = i - leftOffset - 1;
        int right = i + rightOffset + 2;
        int top = roof - 1;
        int bottom = floor + 2;

        PlaceSides(left, right, top, bottom, i, woodBlock, plankedWall);
        PlaceBGWalls(left, right, top, bottom, woodBlock, plankedWall);
        PlaceDoorsAndBeams(left, right, top, bottom, woodBlock, woodenBeam);
        PlacePaintings(left, right, top, bottom, i);
        PlaceMoreBeams(left, right, top, bottom, woodenBeam, plankedWall);
        PlaceDecor(left, right, top, bottom, plankedWall);
        PlaceLargePiles(left, right, top, bottom, woodBlock, plankedWall);
        PlaceSmallPiles(left, right, top, bottom);
        PlaceChandeliers(left, right, top, bottom);
        return true;
    }

    private void PlaceRoofAndFloor(int i, int j, int roofOffset, int floorOffset, int leftOffset, int rightOffset, int roof, int floor, ushort woodBlock, ushort plankedWall) {
        for (int k = 0; k < 2; k++) {
            bool flag = true;
            int i2 = i;
            int j2 = j;
            int directionX = -1;
            int magnitudeX = leftOffset;
            if (k == 1) {
                directionX = 1;
                magnitudeX = rightOffset;
                i2++;
            }
            while (flag) {
                if (j2 - roofOffset < roof) {
                    roof = j2 - roofOffset;
                }
                if (j2 + floorOffset > floor) {
                    floor = j2 + floorOffset;
                }
                for (int l = 0; l < 2; l++) {
                    int j3 = j2;
                    bool flag2 = true;
                    int magnitudeY = roofOffset;
                    int directionY = -1;
                    if (l == 1) {
                        j3++;
                        magnitudeY = floorOffset;
                        directionY = 1;
                    }
                    while (flag2) {
                        if (i2 != i && Main.tile[i2 - directionX, j3].WallType != 27 && (WorldGen.SolidTile(i2 - directionX, j3) || !Main.tile[i2 - directionX, j3].HasTile || Main.tile[i2 - directionX, j3].IsHalfBlock || Main.tile[i2 - directionX, j3].Slope != 0)) {
                            Main.tile[i2 - directionX, j3].Get<TileWallWireStateData>().HasTile = true;
                            Main.tile[i2 - directionX, j3].TileType = woodBlock;
                        }
                        if (WorldGen.SolidTile(i2 - 1, j3) || Main.tile[i2 - 1, j3].IsHalfBlock || Main.tile[i2 - 1, j3].Slope != 0) {
                            Main.tile[i2 - 1, j3].TileType = woodBlock;
                        }
                        if (WorldGen.SolidTile(i2 + 1, j3) || Main.tile[i2 + 1, j3].IsHalfBlock || Main.tile[i2 + 1, j3].Slope != 0) {
                            Main.tile[i2 + 1, j3].TileType = woodBlock;
                        }
                        if (WorldGen.SolidTile(i2, j3) || Main.tile[i2, j3].IsHalfBlock || Main.tile[i2, j3].Slope != 0) {
                            int nearbyTiles = 0;
                            if (WorldGen.SolidTile(i2 - 1, j3)) {
                                nearbyTiles++;
                            }
                            if (WorldGen.SolidTile(i2 + 1, j3)) {
                                nearbyTiles++;
                            }
                            if (WorldGen.SolidTile(i2, j3 - 1)) {
                                nearbyTiles++;
                            }
                            if (WorldGen.SolidTile(i2, j3 + 1)) {
                                nearbyTiles++;
                            }
                            if (nearbyTiles < 2) {
                                Main.tile[i2, j3].Get<TileWallWireStateData>().HasTile = false;
                            }
                            else {
                                flag2 = false;
                                Main.tile[i2, j3].TileType = woodBlock;
                            }
                        }
                        else {
                            Main.tile[i2, j3].WallType = plankedWall;
                            Main.tile[i2, j3].LiquidAmount = 0;
                            Main.tile[i2, j3].Get<LiquidData>().LiquidType = LiquidID.Water;
                        }
                        j3 += directionY;
                        magnitudeY--;
                        if (magnitudeY <= 0) {
                            if (!Main.tile[i2, j3].HasTile) {
                                Main.tile[i2, j3].Get<TileWallWireStateData>().HasTile = true;
                                Main.tile[i2, j3].TileType = woodBlock;
                            }
                            flag2 = false;
                        }
                    }
                }
                magnitudeX--;
                i2 += directionX;
                if (WorldGen.SolidTile(i2, j2)) {
                    int num15 = 0;
                    int num16 = 0;
                    int num17 = j2;
                    bool flag3 = true;
                    while (flag3) {
                        num17--;
                        num15++;
                        if (WorldGen.SolidTile(i2 - directionX, num17)) {
                            num15 = 999;
                            flag3 = false;
                        }
                        else if (!WorldGen.SolidTile(i2, num17)) {
                            flag3 = false;
                        }
                    }
                    num17 = j2;
                    flag3 = true;
                    while (flag3) {
                        num17++;
                        num16++;
                        if (WorldGen.SolidTile(i2 - directionX, num17)) {
                            num16 = 999;
                            flag3 = false;
                        }
                        else if (!WorldGen.SolidTile(i2, num17)) {
                            flag3 = false;
                        }
                    }
                    if (num16 <= num15) {
                        if (num16 > floorOffset) {
                            magnitudeX = 0;
                        }
                        else {
                            j2 += num16 + 1;
                        }
                    }
                    else if (num15 > roofOffset) {
                        magnitudeX = 0;
                    }
                    else {
                        j2 -= num15 + 1;
                    }
                }
                if (magnitudeX <= 0) {
                    flag = false;
                }
            }
        }
    }

    private void PlaceSides(int left, int right, int top, int bottom, int i, ushort woodBlock, ushort plankedWall) {
        for (int m = left; m < right; m++) {
            for (int n = top; n < bottom; n++) {
                if (Main.tile[m, n].WallType == plankedWall && !Main.tile[m, n].HasTile) {
                    if (Main.tile[m - 1, n].WallType != plankedWall && m < i && !WorldGen.SolidTile(m - 1, n)) {
                        WorldGen.PlaceTile(m, n, woodBlock, mute: true);
                        Main.tile[m, n].WallType = 0;
                    }
                    if (Main.tile[m + 1, n].WallType != plankedWall && m > i && !WorldGen.SolidTile(m + 1, n)) {
                        WorldGen.PlaceTile(m, n, woodBlock, mute: true);
                        Main.tile[m, n].WallType = 0;
                    }
                    for (int num22 = m - 1; num22 <= m + 1; num22++) {
                        for (int num23 = n - 1; num23 <= n + 1; num23++) {
                            if (WorldGen.SolidTile(num22, num23)) {
                                Main.tile[num22, num23].TileType = woodBlock;
                            }
                        }
                    }
                }
                if (Main.tile[m, n].TileType == woodBlock && Main.tile[m - 1, n].WallType == plankedWall && Main.tile[m + 1, n].WallType == plankedWall && (Main.tile[m, n - 1].WallType == 27 || Main.tile[m, n - 1].HasTile) && (Main.tile[m, n + 1].WallType == 27 || Main.tile[m, n + 1].HasTile)) {
                    Main.tile[m, n].Get<TileWallWireStateData>().HasTile = false;
                    Main.tile[m, n].WallType = plankedWall;
                }
            }
        }
    }

    private void PlaceBGWalls(int left, int right, int top, int bottom, ushort woodBlock, ushort plankedWall) {
        for (int k = left; k < right; k++) {
            for (int l = top; l < bottom; l++) {
                if (Main.tile[k, l].TileType == woodBlock) {
                    if (Main.tile[k - 1, l].WallType == plankedWall && Main.tile[k + 1, l].WallType == 27 && !Main.tile[k - 1, l].HasTile && !Main.tile[k + 1, l].HasTile) {
                        Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[k, l].WallType = plankedWall;
                    }
                    if (TileID.Sets.IsAContainer[Main.tile[k, l - 1].TileType] && Main.tile[k - 1, l].WallType == 27 && Main.tile[k + 1, l].TileType == woodBlock && Main.tile[k + 2, l].WallType == plankedWall && !Main.tile[k - 1, l].HasTile && !Main.tile[k + 2, l].HasTile) {
                        Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[k, l].WallType = plankedWall;
                        Main.tile[k + 1, l].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[k + 1, l].WallType = plankedWall;
                    }
                    if (Main.tile[k, l - 1].WallType == plankedWall && Main.tile[k, l + 1].WallType == plankedWall && !Main.tile[k, l - 1].HasTile && !Main.tile[k, l + 1].HasTile) {
                        Main.tile[k, l].Get<TileWallWireStateData>().HasTile = false;
                        Main.tile[k, l].WallType = plankedWall;
                    }
                }
            }
        }
    }

    private void PlaceDoorsAndBeams(int left, int right, int top, int bottom, ushort woodBlock, ushort woodenBeam) {
        for (int k = left; k < right; k++) {
            for (int l = bottom; l > top; l--) {
                bool flag4 = false;
                if (Main.tile[k, l].HasTile && Main.tile[k, l].TileType == woodBlock) {
                    int num28 = -1;
                    for (int num29 = 0; num29 < 2; num29++) {
                        if (!WorldGen.SolidTile(k + num28, l) && Main.tile[k + num28, l].WallType == 0) {
                            int num30 = 0;
                            int num31 = l;
                            int num32 = l;
                            while (Main.tile[k, num31].HasTile && Main.tile[k, num31].TileType == woodBlock && !WorldGen.SolidTile(k + num28, num31) && Main.tile[k + num28, num31].WallType == 0) {
                                num31--;
                                num30++;
                            }
                            num31++;
                            int num33 = num31 + 1;
                            if (num30 > 4) {
                                if (Random.NextBool(2)) {
                                    num31 = num32 - 1;
                                    bool flag5 = true;
                                    for (int num34 = k - 2; num34 <= k + 2; num34++) {
                                        for (int num35 = num31 - 2; num35 <= num31; num35++) {
                                            if (num34 != k && Main.tile[num34, num35].HasTile) {
                                                flag5 = false;
                                            }
                                        }
                                    }
                                    if (flag5) {
                                        Main.tile[k, num31].Get<TileWallWireStateData>().HasTile = false;
                                        Main.tile[k, num31 - 1].Get<TileWallWireStateData>().HasTile = false;
                                        Main.tile[k, num31 - 2].Get<TileWallWireStateData>().HasTile = false;
                                        WorldGen.PlaceTile(k, num31, TileID.ClosedDoor, mute: true);
                                        flag4 = true;
                                    }
                                }
                                if (!flag4) {
                                    for (int num36 = num33; num36 < num32; num36++) {
                                        Main.tile[k, num36].TileType = woodenBeam;
                                    }
                                }
                            }
                        }
                        num28 = 1;
                    }
                }
                if (flag4) {
                    break;
                }
            }
        }
    }

    private void PlacePaintings(int left, int right, int top, int bottom, int i) {
        int pictureCount = Random.Next(1, 2);
        if (Random.NextBool(4)) {
            pictureCount = 0;
        }
        if (Random.NextBool(6)) {
            pictureCount++;
        }
        if (Random.NextBool(10)) {
            pictureCount++;
        }

        for (int pictureAttempts = 0; pictureAttempts < pictureCount; pictureAttempts++) {
            int num39 = 0;
            int num40 = Random.Next(left, right);
            int num41 = Random.Next(top, bottom);
            while (!Main.wallHouse[Main.tile[num40, num41].WallType] || Main.tile[num40, num41].HasTile) {
                num39++;
                if (num39 > 1000) {
                    break;
                }
                num40 = Random.Next(left, right);
                num41 = Random.Next(top, bottom);
            }
            if (num39 > 1000) {
                break;
            }
            int num43;
            int num44;
            int num45;
            int num46;
            for (int num42 = 0; num42 < 2; num42++) {
                num43 = num40;
                num44 = num40;
                while (!Main.tile[num43, num41].HasTile && Main.wallHouse[Main.tile[num43, num41].WallType]) {
                    num43--;
                }
                num43++;
                for (; !Main.tile[num44, num41].HasTile && Main.wallHouse[Main.tile[num44, num41].WallType]; num44++) {
                }
                num44--;
                i = (num43 + num44) / 2;
                num45 = num41;
                num46 = num41;
                while (!Main.tile[num40, num45].HasTile && Main.wallHouse[Main.tile[num40, num45].WallType]) {
                    num45--;
                }
                num45++;
                for (; !Main.tile[num40, num46].HasTile && Main.wallHouse[Main.tile[num40, num46].WallType]; num46++) {
                }
                num46--;
                num41 = (num45 + num46) / 2;
            }
            num43 = num40;
            num44 = num40;
            while (!Main.tile[num43, num41].HasTile && !Main.tile[num43, num41 - 1].HasTile && !Main.tile[num43, num41 + 1].HasTile) {
                num43--;
            }
            num43++;
            for (; !Main.tile[num44, num41].HasTile && !Main.tile[num44, num41 - 1].HasTile && !Main.tile[num44, num41 + 1].HasTile; num44++) {
            }
            num44--;
            num45 = num41;
            num46 = num41;
            while (!Main.tile[num40, num45].HasTile && !Main.tile[num40 - 1, num45].HasTile && !Main.tile[num40 + 1, num45].HasTile) {
                num45--;
            }
            num45++;
            for (; !Main.tile[num40, num46].HasTile && !Main.tile[num40 - 1, num46].HasTile && !Main.tile[num40 + 1, num46].HasTile; num46++) {
            }
            num46--;
            num40 = (num43 + num44) / 2;
            num41 = (num45 + num46) / 2;
            int num47 = num44 - num43;
            int num48 = num46 - num45;
            if (num47 <= 7 || num48 <= 5) {
                continue;
            }
            int num49 = 0;
            if (WorldGen.nearPicture2(i, num41)) {
                num49 = -1;
            }
            if (num49 == 0) {
                var housePicture = WorldGen.RandHousePicture();
                int type = housePicture.tileType;
                int style = housePicture.style;
                if (!WorldGen.nearPicture(num40, num41)) {
                    WorldGen.PlaceTile(num40, num41, type, mute: true, forced: false, -1, style);
                }
            }
        }
    }

    private void PlaceMoreBeams(int left, int right, int top, int bottom, ushort woodenBeam, ushort plankedWall) {
        int num50;
        for (num50 = left; num50 < right; num50++) {
            bool flag6 = true;
            for (int num51 = top; num51 < bottom; num51++) {
                for (int num52 = num50 - 3; num52 <= num50 + 3; num52++) {
                    if (Main.tile[num52, num51].HasTile && (!WorldGen.SolidTile(num52, num51) || Main.tile[num52, num51].TileType == TileID.ClosedDoor)) {
                        flag6 = false;
                    }
                }
            }
            if (flag6) {
                for (int num53 = top; num53 < bottom; num53++) {
                    if (Main.tile[num50, num53].WallType == plankedWall && !Main.tile[num50, num53].HasTile) {
                        WorldGen.PlaceTile(num50, num53, woodenBeam, mute: true);
                    }
                }
            }
            num50 += Random.Next(4);
        }
    }

    private void PlaceDecor(int left, int right, int top, int bottom, ushort plankedWall) {
        for (int decorAmount = 0; decorAmount < 4; decorAmount++) {
            int decorX = Random.Next(left + 2, right - 1);
            int decorY = Random.Next(top + 2, bottom - 1);
            while (Main.tile[decorX, decorY].WallType != plankedWall) {
                decorX = Random.Next(left + 2, right - 1);
                decorY = Random.Next(top + 2, bottom - 1);
            }
            while (Main.tile[decorX, decorY].HasTile) {
                decorY--;
            }
            for (; !Main.tile[decorX, decorY].HasTile; decorY++) {
            }
            decorY--;
            if (Main.tile[decorX, decorY].WallType != plankedWall) {
                continue;
            }
            if (Random.NextBool(3)) {
                int decorTile = Random.Next(9) switch {
                    0 => 14,
                    1 => 16,
                    2 => 18,
                    3 => 86,
                    4 => 87,
                    5 => 94,
                    6 => 101,
                    7 => 104,
                    _ => 106,
                };
                WorldGen.PlaceTile(decorX, decorY, decorTile, mute: true);
            }
            else {
                int style2 = Random.Next(2, 43);
                WorldGen.PlaceTile(decorX, decorY, TileID.Statues, mute: true, forced: true, -1, style2);
            }
        }
    }

    private void PlaceLargePiles(int left, int right, int top, int bottom, ushort woodBlock, ushort plankedWall) {
        for (int randomPileAttempts = 0; randomPileAttempts < 40; randomPileAttempts++) {
            int randomPileX = Random.Next(left + 2, right - 1);
            int randomPileY = Random.Next(top + 2, bottom - 1);
            while (Main.tile[randomPileX, randomPileY].WallType != plankedWall) {
                randomPileX = Random.Next(left + 2, right - 1);
                randomPileY = Random.Next(top + 2, bottom - 1);
            }
            while (Main.tile[randomPileX, randomPileY].HasTile) {
                randomPileY--;
            }
            for (; !Main.tile[randomPileX, randomPileY].HasTile; randomPileY++) {
            }
            randomPileY--;
            if (Main.tile[randomPileX, randomPileY].WallType == plankedWall && Random.NextBool(2)) {
                int style3 = Random.Next(22, 26);
                WorldGen.PlaceTile(randomPileX, randomPileY, TileID.LargePiles, mute: true, forced: false, -1, style3);
            }
        }
    }

    private void PlaceSmallPiles(int left, int right, int top, int bottom) {
        for (int smallPileAttempts = 0; smallPileAttempts < 20; smallPileAttempts++) {
            int randomPileX = Random.Next(left + 2, right - 1);
            int randomPileY = Random.Next(top + 2, bottom - 1);
            while (Main.tile[randomPileX, randomPileY].WallType != 27) {
                randomPileX = Random.Next(left + 2, right - 1);
                randomPileY = Random.Next(top + 2, bottom - 1);
            }
            while (Main.tile[randomPileX, randomPileY].HasTile) {
                randomPileY--;
            }
            for (; !Main.tile[randomPileX, randomPileY].HasTile; randomPileY++) {
            }
            randomPileY--;
            if (Main.tile[randomPileX, randomPileY].WallType == 27 && Random.NextBool(2)) {
                int pileStyle = Random.Next(31, 34);
                WorldGen.PlaceSmallPile(randomPileX, randomPileY, pileStyle, 1);
            }
        }
    }

    private void PlaceChandeliers(int left, int right, int top, int bottom) {
        for (int chandelierAttempts = 0; chandelierAttempts < 15; chandelierAttempts++) {
            int chandelierX = Random.Next(left + 2, right - 1);
            int chandelierY = Random.Next(top + 2, bottom - 1);
            while (Main.tile[chandelierX, chandelierY].WallType != 27) {
                chandelierX = Random.Next(left + 2, right - 1);
                chandelierY = Random.Next(top + 2, bottom - 1);
            }
            while (Main.tile[chandelierX, chandelierY].HasTile) {
                chandelierY--;
            }
            while (chandelierY > 0 && !Main.tile[chandelierX, chandelierY - 1].HasTile) {
                chandelierY--;
            }
            if (Main.tile[chandelierX, chandelierY].WallType != 27) {
                continue;
            }
            if (Random.Next(10) < 9) {
                continue;
            }

            int chandelierTileType = TileID.Chandeliers;
            int chandelierStyle = Random.Next(6);
            WorldGen.PlaceTile(chandelierX, chandelierY, chandelierTileType, mute: true, forced: false, -1, chandelierStyle);

            if (Main.tile[chandelierX, chandelierY].TileType != chandelierTileType) {
                continue;
            }

            //if (chandelierTileType == TileID.Torches) {
            //    Tile tile = Main.tile[chandelierX, chandelierY];
            //    tile.TileFrameX += 54;
            //    continue;
            //}

            int oldChandelierX = chandelierX;
            int oldChandelierY = chandelierY;
            chandelierY = oldChandelierY - Main.tile[oldChandelierX, oldChandelierY].TileFrameY / 18;
            chandelierX = Main.tile[oldChandelierX, oldChandelierY].TileFrameX / 18;
            if (chandelierX > 2) {
                chandelierX -= 3;
            }
            chandelierX = oldChandelierX - chandelierX;
            short num71 = 54;
            if (Main.tile[chandelierX, chandelierY].TileFrameX > 0) {
                num71 = -54;
            }
            for (int num72 = chandelierX; num72 < chandelierX + 3; num72++) {
                for (int num73 = chandelierY; num73 < chandelierY + 3; num73++) {
                    Tile tile2 = Main.tile[num72, num73];
                    tile2.TileFrameX += num71;
                }
            }
        }
    }
}