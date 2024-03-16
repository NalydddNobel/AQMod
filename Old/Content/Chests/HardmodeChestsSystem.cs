using Aequus;
using Aequus.Common.Tiles;
using Aequus.Content.DataSets;
using System.Collections.Generic;

namespace Aequus.Old.Content.Chests;

/*public class HardmodeChestsSystem : ModSystem {
    public static readonly Dictionary<TileKey, TileKey> CountsAsChest = new();
    public readonly List<JsonLootDefinition> HardmodeChestLoot = new();
    public readonly List<JsonLootDefinition> HardmodeSnowChestLoot = new();
    public readonly List<JsonLootDefinition> HardmodeJungleChestLoot = new();

    public record struct OreTierData(int Tile, int Ore, int Bar);

    public readonly Dictionary<int, OreTierData> TileIDToOreTier = new();

    public override void Load() {
        CountsAsChest[new(TileID.Containers, ChestType.Marble)] = new(TileID.Containers, ChestType.Gold);
        CountsAsChest[new(TileID.Containers, ChestType.Granite)] = new(TileID.Containers, ChestType.Gold);
        CountsAsChest[new(TileID.Containers, ChestType.Mushroom)] = new(TileID.Containers, ChestType.Gold);
        CountsAsChest[new(TileID.Containers, ChestType.RichMahogany)] = new(TileID.Containers, ChestType.Ivy);
        HardmodeChestLoot.Add(ItemID.DualHook);
        HardmodeChestLoot.Add(ItemID.MagicDagger);
        HardmodeChestLoot.Add(ItemID.TitanGlove);
        HardmodeChestLoot.Add(ItemID.PhilosophersStone);
        HardmodeChestLoot.Add(ItemID.CrossNecklace);
        HardmodeChestLoot.Add(ItemID.StarCloak);
        HardmodeSnowChestLoot = new List<int>() {
            ItemID.Frostbrand,
            ItemID.IceBow,
            ItemID.FlowerofFrost,
        };
        TileIDToOreTier = new Dictionary<int, OreTierData>() {
            [TileID.Cobalt] = new OreTierData(TileID.Cobalt, ItemID.CobaltOre, ItemID.CobaltBar),
            [TileID.Mythril] = new OreTierData(TileID.Mythril, ItemID.MythrilOre, ItemID.MythrilBar),
            [TileID.Adamantite] = new OreTierData(TileID.Adamantite, ItemID.AdamantiteOre, ItemID.AdamantiteBar),
            [TileID.Palladium] = new OreTierData(TileID.Palladium, ItemID.PalladiumOre, ItemID.PalladiumBar),
            [TileID.Orichalcum] = new OreTierData(TileID.Orichalcum, ItemID.OrichalcumOre, ItemID.OrichalcumBar),
            [TileID.Titanium] = new OreTierData(TileID.Titanium, ItemID.TitaniumOre, ItemID.TitaniumBar),
        };
    }

    public override void Unload() {
        CountsAsChest.Clear();
        HardmodeJungleChestLoot?.Clear();
        HardmodeJungleChestLoot = null;
        HardmodeChestLoot?.Clear();
        HardmodeChestLoot = null;
        HardmodeSnowChestLoot?.Clear();
        HardmodeSnowChestLoot = null;
        TileIDToOreTier?.Clear();
        TileIDToOreTier = null;
    }

    private static OreTierData GetOreDataFromTileOrDefault(int tileID, int defaultTileID) {
        if (TileIDToOreTier.TryGetValue(tileID, out var val))
            return val;
        return TileIDToOreTier[defaultTileID];
    }

    private static void ReplaceChestBarsAndOres(int barItem, OreTierData oreData) {
        for (int i = 0; i < ChestOpenedTracker.UnopenedChests.Count; i++) {
            int chestID = Chest.FindChest(ChestOpenedTracker.UnopenedChests[i].X, ChestOpenedTracker.UnopenedChests[i].Y);
            if (chestID > -1 && ChestOpenedTracker.IsRealChest(chestID) && ChestType.isGenericUndergroundChest(Main.chest[chestID])) {
                for (int k = 0; k < Chest.maxItems; k++) {
                    if (Main.chest[chestID].item[k].type == barItem) {
                        int stack = Main.chest[chestID].item[k].stack;
                        Main.chest[chestID].item[k].SetDefaults(oreData.Bar);
                        Main.chest[chestID].item[k].stack = stack;
                    }
                }
            }
        }
    }

    private static bool Crimson() {
        if (Main.drunkWorld)
            return WorldGen.genRand.NextBool();
        return WorldGen.crimson;
    }

    public static void HardmodifyAnItem(Chest chest, int i) {
        var item = chest.item[i];
        if (item.type == ItemID.FlamingArrow || item.type == ItemID.WoodenArrow) {
            if (Crimson()) {
                item.SetDefaults(ItemID.IchorArrow);
                item.stack = WorldGen.genRand.Next(50, 100);
            }
            else {
                item.SetDefaults(ItemID.CursedArrow);
                item.stack = WorldGen.genRand.Next(50, 100);
            }
        }
        else if (item.type == ItemID.ThrowingKnife || item.type == ItemID.Shuriken || item.type == ItemID.Flare) {
            if (Crimson()) {
                item.SetDefaults(ItemID.IchorBullet);
                item.stack = WorldGen.genRand.Next(50, 100);
            }
            else {
                item.SetDefaults(ItemID.CursedBullet);
                item.stack = WorldGen.genRand.Next(50, 100);
            }
        }
        else if (item.type == ItemID.RecallPotion) {
            item.SetDefaults(ItemID.TeleportationPotion);
        }
        else if (item.type == ItemID.Glowstick) {
            int stack = item.stack;
            item.SetDefaults(ItemID.SpelunkerGlowstick);
            item.stack = stack;
        }
        else if (item.type == ItemID.LesserHealingPotion) {
            int stack = item.stack;
            item.SetDefaults(ItemID.HealingPotion);
            item.stack = stack;
        }
        else if (item.type == ItemID.HealingPotion) {
            int stack = item.stack;
            item.SetDefaults(ItemID.GreaterHealingPotion);
            item.stack = stack;
        }
        else if (item.type == ItemID.SuspiciousLookingEye || item.type == ItemID.SlimeCrown) {
            item.SetDefaults(Main.rand.NextFromList(ItemID.MechanicalEye, ItemID.MechanicalSkull, ItemID.MechanicalWorm));
        }
    }
    public static void TryHardmodifyAPotion(Chest chest, int i) {
        var item = chest.item[i];
        switch (WorldGen.genRand.Next(3)) {
            case 0:
                if (ModContent.GetInstance<BoundedPrefix>().CanRoll(item)) {
                    int stack = item.stack;
                    item.SetDefaults(item.type);
                    item.stack = stack;
                    item.Prefix(ModContent.PrefixType<BoundedPrefix>());
                }
                break;

            case 1:
                if (ModContent.GetInstance<DoubledTimePrefix>().CanRoll(item)) {
                    int stack = item.stack;
                    item.SetDefaults(item.type);
                    item.stack = stack;
                    item.Prefix(ModContent.PrefixType<DoubledTimePrefix>());
                }
                break;

            case 2:
                if (ModContent.GetInstance<EmpoweredPrefix>().CanRoll(item)) {
                    int stack = item.stack;
                    item.SetDefaults(item.type);
                    item.stack = stack;
                    item.Prefix(ModContent.PrefixType<EmpoweredPrefix>());
                }
                break;
        }
    }
    public static void HardmodifyPreHardmodeLoot(Chest chest) {
        for (int i = 0; i < Chest.maxItems; i++) {
            var item = chest.item[i];
            HardmodifyAnItem(chest, i);
            TryHardmodifyAPotion(chest, i);
        }
    }
    public static void AddGenericLoot(Chest chest) {
        switch (WorldGen.genRand.Next(5)) {
            case 0:
                chest.AddItem(ItemID.SoulofLight, WorldGen.genRand.Next(3, 12));
                break;
            case 1:
                chest.AddItem(ItemID.SoulofNight, WorldGen.genRand.Next(3, 12));
                break;
            case 2:
                chest.AddItem(Crimson() ? ItemID.Ichor : ItemID.CursedFlame, WorldGen.genRand.Next(5, 20));
                break;
            case 3:
                chest.AddItem(ItemID.PixieDust, WorldGen.genRand.Next(16, 30));
                break;
        }
    }
    public static void AddMainLoot(Chest chest, int tileID, int chestStyle) {
        if (tileID == TileID.Containers) {
            switch (chestStyle) {
                case ChestType.Gold:
                case ChestType.Marble:
                case ChestType.Granite:
                case ChestType.Mushroom:
                    chest.item[0].SetDefaults(WorldGen.genRand.Next(HardmodeChestLoot));
                    break;

                case ChestType.Frozen:
                    chest.item[0].SetDefaults(WorldGen.genRand.Next(HardmodeSnowChestLoot));
                    break;

                case ChestType.RichMahogany:
                case ChestType.Ivy:
                    chest.item[0].SetDefaults(WorldGen.genRand.Next(HardmodeJungleChestLoot));
                    break;
            }
        }
    }
    public static void AddSecondaryLoot(Chest chest, int tileID, int chestStyle) {
        if (tileID == TileID.Containers) {
            switch (chestStyle) {
                case ChestType.Frozen:
                    if (WorldGen.genRand.NextBool(3)) {
                        chest.AddItem(ItemID.FrostCore);
                    }
                    break;

                case ChestType.RichMahogany:
                case ChestType.Ivy:
                    if (WorldGen.genRand.NextBool(3)) {
                        chest.AddItem(ItemID.TurtleShell);
                    }
                    break;
            }
        }
        else if (tileID == TileID.Containers2 && chestStyle == ChestType.Sandstone) {
            if (WorldGen.genRand.NextBool(3)) {
                chest.AddItem(ItemID.AncientBattleArmorMaterial);
            }
        }
    }
    public static void Hardmodify(Chest chest) {
        int chestStyleOriginal = ChestType.GetStyle(chest);
        int tileIDOriginal = Main.tile[chest.x, chest.y].TileType;
        int chestStyle = chestStyleOriginal;
        int tileID = tileIDOriginal;

        if (CountsAsChest.TryGetValue(new(tileID, chestStyle), out var replaceKey)) {
            tileID = replaceKey.TileType;
            chestStyle = replaceKey.TileStyle;
        }

        HardmodifyPreHardmodeLoot(chest);
        AddMainLoot(chest, tileID, chestStyle);

        AddSecondaryLoot(chest, tileID, chestStyle);
        for (int i = 0; i < 2; i++) {
            if (WorldGen.genRand.NextBool()) {
                AddGenericLoot(chest);
            }
        }

        ChangeChestToHardmodeVariant(chest, chestStyleOriginal, tileIDOriginal);
        chest.SquishAndStackContents();
    }
    public static void ChangeChestToHardmodeVariant(Chest chest, int chestStyle, int tileID) {
        if (tileID == TileID.Containers) {
            switch (chestStyle) {
                case ChestType.Gold: {
                        InnerChangeChestToHardmodeVariant<AdamantiteChestTile>(chest.x, chest.y);
                    }
                    break;
                case ChestType.Frozen: {
                        InnerChangeChestToHardmodeVariant<HardFrozenChestTile>(chest.x, chest.y);
                    }
                    break;
                case ChestType.Granite: {
                        InnerChangeChestToHardmodeVariant<HardGraniteChestTile>(chest.x, chest.y);
                    }
                    break;
                case ChestType.RichMahogany:
                case ChestType.Ivy: {
                        InnerChangeChestToHardmodeVariant<HardJungleChestTile>(chest.x, chest.y);
                    }
                    break;
                case ChestType.Marble: {
                        InnerChangeChestToHardmodeVariant<HardMarbleChestTile>(chest.x, chest.y);
                    }
                    break;
                case ChestType.Mushroom: {
                        InnerChangeChestToHardmodeVariant<HardMushroomChestTile>(chest.x, chest.y);
                    }
                    break;
                case ChestType.Webbed: {
                        InnerChangeChestToHardmodeVariant(chest.x, chest.y, TileID.Containers2, ChestType.Spider);
                    }
                    break;
            }
        }
        else if (tileID == TileID.Containers2) {
            switch (chestStyle) {
                case ChestType.Sandstone: {
                        InnerChangeChestToHardmodeVariant<HardSandstoneChestTile>(chest.x, chest.y);
                    }
                    break;
            }
        }
    }
    public static void InnerChangeChestToHardmodeVariant<T>(int x, int y) where T : ModTile {
        x -= Main.tile[x, y].TileFrameX % 36 / 18;
        y -= Main.tile[x, y].TileFrameY % 36 / 18;
        var tileType = (ushort)ModContent.TileType<T>();
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                Main.tile[x + i, y + j].Active(value: true);
                Main.tile[x + i, y + j].TileType = tileType;
                Main.tile[x + i, y + j].TileFrameX = (short)(i * 18);
                Main.tile[x + i, y + j].TileFrameY = (short)(j * 18);
            }
        }
        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendTileSquare(-1, x, y, 2, 2);
        }
    }
    public static void InnerChangeChestToHardmodeVariant(int x, int y, ushort tileType, int style) {
        x -= Main.tile[x, y].TileFrameX % 36 / 18;
        y -= Main.tile[x, y].TileFrameY % 36 / 18;
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                Main.tile[x + i, y + j].Active(value: true);
                Main.tile[x + i, y + j].TileType = tileType;
                Main.tile[x + i, y + j].TileFrameX = (short)((i + style * 2) * 18);
                Main.tile[x + i, y + j].TileFrameY = (short)(j * 18);
            }
        }
        if (Main.netMode != NetmodeID.SinglePlayer) {
            NetMessage.SendTileSquare(-1, x, y, 2, 2);
        }
    }

    public override void PostUpdateWorld() {
        if (!Main.hardMode || WorldGen.IsGeneratingHardMode || Main.netMode == NetmodeID.MultiplayerClient || !GameplayConfig.Instance.HardmodeChests) {
            return;
        }

        if (!AequusWorld.hardmodeChests && WorldGen.SavedOreTiers.Cobalt > 0) {
            for (int k = 0; k < Main.maxTilesX + Main.maxTilesY; k++) {
                int x = WorldGen.genRand.Next(100, Main.maxTilesX);
                int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer - 50);
                if (!Main.wallHouse[Main.tile[x, y].WallType] && !Main.tile[x, y].NoDungeonOrTempleWall() && Main.tile[x, y].WallType != WallID.HiveUnsafe) {
                    var r = new Rectangle(x - 5, y - 5, 10, 10);
                    int style = -1;
                    int chestType = TileID.Containers;
                    if (TileHelper.ScanTiles(r, TileHelper.HasTileAction(TileID.MushroomGrass))) {
                        style = ChestType.Mushroom;
                    }
                    else if (TileHelper.ScanTiles(r, TileHelper.HasTileAction(TileID.JungleGrass, TileID.LihzahrdBrick, TileID.Hive, TileID.RichMahogany, TileID.LivingMahogany, TileID.LivingMahoganyLeaves))) {
                        style = ChestType.Ivy;
                    }
                    else if (TileHelper.ScanTiles(r, TileHelper.HasWallAction(WallID.GraniteUnsafe, WallID.Granite, WallID.GraniteBlock))) {
                        style = ChestType.Granite;
                    }
                    else if (TileHelper.ScanTiles(r, TileHelper.HasWallAction(WallID.MarbleUnsafe, WallID.Marble, WallID.MarbleBlock))) {
                        style = ChestType.Marble;
                    }
                    if (!WorldGen.AddBuriedChest(new Point(x, y), notNearOtherChests: true, Style: style))
                        continue;
                    for (int l = y - 8; l < Main.maxTilesY - 10; l++) {
                        int guess = Chest.FindChestByGuessing(x, l);
                        if (guess == -1)
                            guess = Chest.FindChestByGuessing(x - 1, l);
                        if (guess != -1) {
                            k += 80;
                            if (style != -1 && Main.tile[Main.chest[guess].x, Main.chest[guess].y].TileType == chestType) {
                                var chestStyle = ChestType.GetStyle(Main.chest[guess]);
                                if (chestStyle != style) {
                                    for (int i = 0; i < 2; i++) {
                                        for (int j = 0; j < 2; j++) {
                                            var tile = Main.tile[Main.chest[guess].x + i, Main.chest[guess].y + j];
                                            tile.TileFrameX = (short)(tile.TileFrameX % 36 + style * 36);
                                            tile.TileFrameY = (short)(tile.TileFrameY % 36 + style * 36);
                                        }
                                    }
                                }
                            }
                            ChestOpenedTracker.UnopenedChests.Add(new Point(Main.chest[guess].x, Main.chest[guess].y));
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < ChestOpenedTracker.UnopenedChests.Count; i++) {
                int chestID = Chest.FindChest(ChestOpenedTracker.UnopenedChests[i].X, ChestOpenedTracker.UnopenedChests[i].Y);
                if (chestID > -1
                    && ChestOpenedTracker.IsRealChest(chestID)
                    && ChestType.isGenericUndergroundChest(Main.chest[chestID])
                    && Main.chest[chestID].FindItem(item =>
                        item != null && !item.IsAir && ItemSets.ImportantItem.Contains(item.type)) == null) {
                    Hardmodify(Main.chest[chestID]);
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        Helper.ChestConversionNetUpdate(chestID);
                    }
                }
            }
            AequusWorld.hardmodeChests = true;
            TextHelper.Broadcast("Announcement.HardmodeChests", TextHelper.EventMessageColor.HueAdd(0.075f));
        }
        else if (!AequusWorld.chestCobaltTier && WorldGen.SavedOreTiers.Cobalt > 0) {
            ReplaceChestBarsAndOres(ItemID.IronBar, GetOreDataFromTileOrDefault(WorldGen.SavedOreTiers.Cobalt, TileID.Cobalt));
            ReplaceChestBarsAndOres(ItemID.LeadBar, GetOreDataFromTileOrDefault(WorldGen.SavedOreTiers.Cobalt, TileID.Cobalt));
            AequusWorld.chestCobaltTier = true;
        }
        else if (!AequusWorld.chestMythrilTier && WorldGen.SavedOreTiers.Mythril > 0) {
            ReplaceChestBarsAndOres(ItemID.SilverBar, GetOreDataFromTileOrDefault(WorldGen.SavedOreTiers.Mythril, TileID.Mythril));
            ReplaceChestBarsAndOres(ItemID.TungstenBar, GetOreDataFromTileOrDefault(WorldGen.SavedOreTiers.Mythril, TileID.Mythril));
            AequusWorld.chestMythrilTier = true;
        }
        else if (!AequusWorld.chestAdamantiteTier && WorldGen.SavedOreTiers.Adamantite > 0) {
            ReplaceChestBarsAndOres(ItemID.GoldBar, GetOreDataFromTileOrDefault(WorldGen.SavedOreTiers.Adamantite, TileID.Adamantite));
            ReplaceChestBarsAndOres(ItemID.PlatinumBar, GetOreDataFromTileOrDefault(WorldGen.SavedOreTiers.Adamantite, TileID.Adamantite));
            AequusWorld.chestAdamantiteTier = true;
        }
    }
}*/