using Aequus.Common.Renaming;
using Aequus.Common.WorldGeneration;
using Aequus.Content.Tiles;
using Aequus.Old.Content.Events.DemonSiege.Tiles;
using Aequus.Old.Content.Tiles.Ambient;
using Aequus.Old.Content.Tiles.Furniture.Oblivion;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Aequus.Old.Content.WorldGeneration;

public class GoreNestsGeneration : AequusGenStep {
    public override string InsertAfter => "Underworld";

    public override void Apply(GenerationProgress progress, GameConfiguration config) {
        SetMessage(progress);

        GetGenerationValues(out int minY, out int maxY, out int wantedGoreNests);
        int goreNestCount = 0;
        int loops = 1000000;
        for (int i = 0; i < loops; i++) {
            SetProgress(progress, i / (double)loops);
            int x = WorldGen.genRand.Next(80, Main.maxTilesX - 80);
            int y = WorldGen.genRand.Next(minY, maxY);
            try {
                if (TryGrowGoreNest(x, y)) {
                    goreNestCount++;
                    loops = 10000;
                    if (goreNestCount > wantedGoreNests) {
                        break;
                    }
                }
            }
            catch {

            }
        }
    }

    public bool[] SafeTile { get; private set; }
    public bool[] SafeWall { get; private set; }

    private void PopulateSets() {
        SafeTile = ExtendArray.CreateArray((i) => true, TileLoader.TileCount);
        SafeWall = ExtendArray.CreateArray((i) => true, WallLoader.WallCount);

        for (int t = 0; t < TileLoader.TileCount; t++) {
            if (Main.tileDungeon[t]) {
                SafeTile[t] = false;
            }
        }
        for (int w = 0; w < WallLoader.WallCount; w++) {
            if (Main.wallDungeon[w]) {
                SafeWall[w] = false;
            }
        }

        SafeTile[TileID.LihzahrdBrick] = false;
        SafeTile[TileID.ObsidianBrick] = false;
        SafeTile[TileID.HellstoneBrick] = false;
        SafeTile[ModContent.TileType<OblivionAltar>()] = false;

        SafeWall[WallID.ObsidianBrick] = false;
        SafeWall[WallID.ObsidianBrickUnsafe] = false;
        SafeWall[WallID.HellstoneBrick] = false;
        SafeWall[WallID.HellstoneBrickUnsafe] = false;
    }

    public static void GetGenerationValues(out int minY, out int maxY, out int wantedGoreNests) {
        if (Main.remixWorld) {
            minY = (int)(Main.worldSurface * 0.2);
            maxY = (int)Main.worldSurface;
        }
        else {
            minY = Main.UnderworldLayer + 75;
            maxY = Main.UnderworldLayer + 150;
        }

        if (Main.zenithWorld) {
            wantedGoreNests = Main.maxTilesX / 300;
        }
        else {
            wantedGoreNests = Main.maxTilesX / 1500;
        }
    }

    private bool TryGrowGoreNest(int x, int y) {
        if (Main.tile[x, y].HasTile) {
            if (!Main.tile[x, y - 1].HasTile) {
                y--;
            }
            else {
                return false;
            }
        }
        else if (!Main.tile[x, y + 1].HasTile) {
            return false;
        }

        var structure = new Rectangle(x - 60, y - 60, 120, 90).Fluffize(5);
        if (!GenVars.structures.CanPlace(structure, SafeTile) || AnyBlacklistedTiles(x, y)) {
            return false;
        }

        y -= 2;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                int x2 = x + i;
                int y2 = y + j;
                Tile tile = Main.tile[x2, y2];
                tile.HasTile = false;
                if (Main.tile[x2, y2].HasTile || Main.tile[x2, y2].LiquidAmount > 0) {
                    return false;
                }
            }
        }
        y += 3;
        for (int i = 0; i < 3; i++) {
            int x2 = x + i;
            if (!Main.tile[x2, y].HasTile || !Main.tileSolid[Main.tile[x2, y].TileType] || Main.tileCut[Main.tile[x2, y].TileType])
                return false;
        }
        for (int i = 0; i < 3; i++) {
            int x2 = x + i;
            Tile tile = Main.tile[x2, y];
            tile.Slope = 0;
            tile.IsHalfBlock = false;
        }
        y--;
        x++;
        WorldGen.PlaceTile(x, y, ModContent.TileType<OblivionAltar>(), mute: true, forced: true);
        if (Main.tile[x, y].TileType != ModContent.TileType<OblivionAltar>()) {
            return false;
        }
        GenerateSurroundingGoreNestHill(x, y);
        GenerateChests(x, y);
        GenerateSigns(x, y);
        GenerateAmbientTiles(x, y);
        GenVars.structures.AddStructure(structure);
        return true;
    }

    private bool AnyBlacklistedTiles(int x, int y) {
        for (int i = x - 25; i < x + 25; i++) {
            for (int j = y - 25; j < y + 25; j++) {
                if (Main.tile[i, j].HasTile && !SafeTile[Main.tile[i, j].TileType]) {
                    return true;
                }
            }
        }
        return false;
    }

    private void GenerateSurroundingGoreNestHill(int x, int y) {
        HillSpawnAsh(x, y);
        int k = 0;
        int maxY = y + 40;
        while (y < maxY) {
            if (WorldGen.genRand.NextBool(3)) {
                y++;
            }

            HillSpawnAsh(x + k + 1, y);
            HillSpawnAsh(x - k - 1, y);
            k++;
            if (k > 45 || k > 20 && WorldGen.genRand.NextBool(15)) {
                break;
            }
        }

        HillTryToSmoothyGoIntoRegularGeneration(x, y, k, 1);
        HillTryToSmoothyGoIntoRegularGeneration(x, y, k, -1);
    }

    private void HillTryToSmoothyGoIntoRegularGeneration(int x, int y, int k, int dir) {
        k *= dir;
        if (y < Main.maxTilesY) {
            y = Main.maxTilesY - 1;
        }
        while ((x + k + dir < 0 || x + k + dir > Main.maxTilesX)
            && !Main.tile[x + k + dir, y].HasTile && !Main.tile[x + k + dir, y - 1].HasTile) {
            k += dir;
            if (WorldGen.genRand.NextBool(3)) {
                y--;
            }
            if (y < Main.UnderworldLayer) {
                break;
            }
            HillSpawnAsh(x + k, y, kill: true);
        }
        x -= dir;
        while (true) {
            x += dir;
            if (x + k < 0 || x + k > Main.maxTilesX) {
                break;
            }
            y += WorldGen.genRand.Next(3);
            if (y > Main.maxTilesY) {
                break;
            }
            HillSpawnAsh(x + k, y, kill: false);
        }
    }

    private void HillSpawnAsh(int x, int y, bool kill = true) {
        int l = 0;
        var tileId = Main.remixWorld ? TileID.Mud : TileID.Ash;
        while (true) {
            l++;
            if (y + l > Main.maxTilesY) {
                break;
            }
            if (l > 75 || (l > 40 && WorldGen.genRand.NextBool(15))) {
                break;
            }

            var tile = Main.tile[x, y + l];
            if (kill && SafeTile[Main.tile[x, y - l].TileType] && SafeWall[Main.tile[x, y - l].WallType]) {
                WorldGen.KillTile(x, y - l, noItem: true);
            }

            tile.LiquidAmount = 0;

            if (!SafeTile[Main.tile[x, y + l].TileType] || !SafeWall[Main.tile[x, y + l].WallType]) {
                continue;
            }

            if (tile.HasTile && Main.tileFrameImportant[tile.TileType]) {
                WorldGen.KillTile(x, y + l);
            }
            tile.HasTile = true;
            tile.TileType = tileId;
            tile.Slope = 0;
            tile.IsHalfBlock = false;
        }
    }

    private static void GenerateChests(int x, int y) {
        if (Main.remixWorld) {
            return;
        }

        var genTangle = new Rectangle(x - 40, y - 20, 80, 40);
        for (int i = 0; i < 1250; i++) {
            var v = WorldGen.genRand.NextVector2FromRectangle(genTangle).ToPoint();
            if (!Main.tile[v.X, v.Y].HasTile) {
                int c = WorldGen.PlaceChest(v.X, v.Y, type: (ushort)ModContent.TileType<OblivionChest>());
                if (c != -1) {
                    FillChest(Main.chest[c]);
                    break;
                }
            }
        }
    }

    private static void GenerateAmbientTiles(int x, int y) {
        var genTangle = new Rectangle(x - 40, y - 20, 80, 40);
        for (int i = 0; i < 1250; i++) {
            var v = WorldGen.genRand.NextVector2FromRectangle(genTangle).ToPoint();
            WorldGen.PlaceTile(v.X, v.Y, ModContent.TileType<GoreNestStalagmite>(), style: WorldGen.genRand.Next(6));
        }
    }

    private void GenerateSigns(int x, int y) {
        var genTangle = new Rectangle(x - 60, y - 20, 120, 40);
        for (int i = 0; i < 1250; i++) {
            var v = WorldGen.genRand.NextVector2FromRectangle(genTangle).ToPoint();
            if (!Main.tile[v.X, v.Y].HasTile) {
                WorldGen.PlaceTile(v.X, v.Y, ModContent.TileType<AshTombstones>(), style: WorldGen.genRand.Next(6));
                if (Main.tile[v.X, v.Y].HasTile) {
                    int sign = Sign.ReadSign(v.X, v.Y);
                    if (sign >= 0) {
                        string text = Language.GetTextValueWith("Mods.Aequus.GoreNestTombstones." + WorldGen.genRand.Next(4), new { Name = Language.GetTextValue("Mods.Aequus.Names." + WorldGen.genRand.Next(10)) });
                        Sign.TextSign(sign, text + Language.GetTextValue("Mods.Aequus.GoreNestTombstones.Hint." + WorldGen.genRand.Next(6)));
                    }
                    i += 400;
                }
            }
        }
    }

    private static void FillChest(Chest chest) {
        int slot = 0;
        chest.item[slot].SetDefaults(WorldGen.crimson ? ItemID.LightsBane : ItemID.BloodButcherer); // Opposite evil sword
        chest.item[slot++].GetGlobalItem<RenameItem>().CustomName = "$Mods.Aequus.GoreNestTombstones.Names." + WorldGen.genRand.Next(11) + "|$Mods.Aequus.GoreNestTombstones.Sword";
        if (WorldGen.genRand.NextBool()) {
            chest.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.SilverPickaxe, ItemID.TungstenPickaxe, ItemID.GoldPickaxe, ItemID.PlatinumPickaxe));
        }
        if (WorldGen.genRand.NextBool()) {
            chest.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.SilverAxe, ItemID.TungstenAxe, ItemID.GoldAxe, ItemID.PlatinumAxe, ItemID.WarAxeoftheNight, ItemID.BloodLustCluster));
        }
        if (WorldGen.genRand.NextBool(3)) {
            chest.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.SilverHammer, ItemID.TungstenHammer, ItemID.GoldHammer, ItemID.PlatinumHammer));
        }
        chest.item[slot].SetDefaults(ItemID.HealingPotion);
        chest.item[slot++].stack = WorldGen.genRand.Next(10, 25);

        if (WorldGen.genRand.NextBool(3)) {
            chest.item[slot++].SetDefaults(ItemID.ObsidianSkull);
        }

        if (WorldGen.genRand.NextBool(3)) {
            chest.item[slot++].SetDefaults(ItemID.MagicMirror);
        }
        else {
            int recallStack = WorldGen.genRand.Next(10);
            if (recallStack > 0) {
                chest.item[slot].SetDefaults(ItemID.RecallPotion);
                chest.item[slot++].stack = recallStack;
            }
        }

        if (WorldGen.genRand.NextBool()) {
            chest.item[slot++].SetDefaults(Utils.SelectRandom(WorldGen.genRand, ItemID.StarStatue, ItemID.HeartStatue, ItemID.AngelStatue));
        }
    }
}
