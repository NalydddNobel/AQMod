using Aequus.Common.DataSets;
using Aequus.Common.Preferences;
using Aequus.Common.Tiles;
using Aequus.Common.World;
using Aequus.Content.ItemPrefixes.Potions;
using Aequus.CrossMod;
using Aequus.Systems.Chests;
using Aequus.Systems.Chests.DropRules;
using Aequus.Tiles.Furniture.HardmodeChests;
using System.Collections.Generic;

namespace Aequus.Content.World;
public sealed class HardmodeChestBoost : ModSystem {
    public static readonly Dictionary<TileKey, TileKey> CountsAsChest = new();
    public record struct OreTierData(int Tile, int Ore, int Bar);

    public static Dictionary<int, OreTierData> TileIDToOreTier { get; private set; }

    public override void Load() {
        CountsAsChest.Clear();
        CountsAsChest[new(TileID.Containers, ChestType.Marble)] = new(TileID.Containers, ChestType.Gold);
        CountsAsChest[new(TileID.Containers, ChestType.Granite)] = new(TileID.Containers, ChestType.Gold);
        CountsAsChest[new(TileID.Containers, ChestType.Mushroom)] = new(TileID.Containers, ChestType.Gold);
        CountsAsChest[new(TileID.Containers, ChestType.RichMahogany)] = new(TileID.Containers, ChestType.Ivy);
        TileIDToOreTier = new Dictionary<int, OreTierData>() {
            [TileID.Cobalt] = new OreTierData(TileID.Cobalt, ItemID.CobaltOre, ItemID.CobaltBar),
            [TileID.Mythril] = new OreTierData(TileID.Mythril, ItemID.MythrilOre, ItemID.MythrilBar),
            [TileID.Adamantite] = new OreTierData(TileID.Adamantite, ItemID.AdamantiteOre, ItemID.AdamantiteBar),
            [TileID.Palladium] = new OreTierData(TileID.Palladium, ItemID.PalladiumOre, ItemID.PalladiumBar),
            [TileID.Orichalcum] = new OreTierData(TileID.Orichalcum, ItemID.OrichalcumOre, ItemID.OrichalcumBar),
            [TileID.Titanium] = new OreTierData(TileID.Titanium, ItemID.TitaniumOre, ItemID.TitaniumBar),
        };
    }

    public sealed override void SetupContent() {

        #region Loot params
        // Bombs
        int minBombs = 1;
        int maxBombs = 4;
        IChestLootRule bombs = new CommonChestRule(ItemID.Dynamite, minBombs, maxBombs, ChanceDenominator: 3);

        IChestLootRule angelStatue = new CommonChestRule(ItemID.AngelStatue, ChanceDenominator: 5);

        // Metal bars.
        int minBar = 5;
        int maxBar = 14;
        IChestLootRule[] bars = [
            new CommonChestRule(ItemID.CobaltBar, MinStack: minBar, MaxStack: maxBar),
            new CommonChestRule(ItemID.PalladiumBar, MinStack: minBar, MaxStack: maxBar),
            new CommonChestRule(ItemID.MythrilBar, MinStack: minBar, MaxStack: maxBar),
            new CommonChestRule(ItemID.OrichalcumBar, MinStack: minBar, MaxStack: maxBar),
            new CommonChestRule(ItemID.AdamantiteBar, MinStack: minBar, MaxStack: maxBar),
            new CommonChestRule(ItemID.TitaniumBar, MinStack: minBar, MaxStack: maxBar),
        ];
        // Only allow cobalt and palladium if Calamity is enabled.
        if (CalamityMod.IsEnabled()) {
            bars = bars[..1];
        }

        // Ammo.
        int minAmmo = 50;
        int maxAmmo = 199;
        IChestLootRule[] ammo = [
            new CommonChestRule(ItemID.HellfireArrow, MinStack: minAmmo, MaxStack: maxAmmo),
            new CommonChestRule(ItemID.ExplodingBullet, MinStack: minAmmo, MaxStack: maxAmmo),
        ];

        // Healing potions.
        int minHeal = 3;
        int maxHeal = 5;
        IChestLootRule healingPotion = new CommonChestRule(ItemID.GreaterHealingPotion, minHeal, maxHeal, ChanceDenominator: 2);

        // Buff potions.
        int minBuff = 3;
        int maxBuff = 5;
        IChestLootRule[] damageBuffPotions = [
            new CommonChestRule(ItemID.ArcheryPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.MagicPowerPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.ManaRegenerationPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.InfernoPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.WrathPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.RagePotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.SummoningPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.TitanPotion, MinStack: minBuff, MaxStack: maxBuff),
        ];
        IChestLootRule[] healthBuffPotions = [
            new CommonChestRule(ItemID.RegenerationPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.SwiftnessPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.EndurancePotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.LifeforcePotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.IronskinPotion, MinStack: minBuff, MaxStack: maxBuff),
        ];
        IChestLootRule[] utilBuffPotions = [
            new CommonChestRule(ItemID.NightOwlPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.SwiftnessPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.WaterWalkingPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.FeatherfallPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.HunterPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.BiomeSightPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.BattlePotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.CalmingPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.GillsPotion, MinStack: minBuff, MaxStack: maxBuff),
        ];
        IChestLootRule[] miningBuffPotions = [
            new CommonChestRule(ItemID.MiningPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.SpelunkerPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.TrapsightPotion, MinStack: minBuff, MaxStack: maxBuff),
            new CommonChestRule(ItemID.ObsidianSkinPotion, MinStack: minBuff, MaxStack: maxBuff),
        ];

        int minTorch = 15;
        int maxTorch = 29;
        IChestLootRule[] torches = [
            new CommonChestRule(ItemID.Torch, MinStack: minTorch, MaxStack: maxTorch),
            new CommonChestRule(ItemID.Glowstick, MinStack: minTorch, MaxStack: maxTorch),
        ];

        IChestLootRule money = new CommonChestRule(ItemID.GoldCoin, 1, 5, ChanceDenominator: 2);
        #endregion

        #region Regular loot
        ChestLootDatabase.Instance.RegisterIndexed(1, ChestPool.UndergroundHard, [
            new CommonChestRule(ItemID.DualHook),
            new CommonChestRule(ItemID.MagicDagger),
            new CommonChestRule(ItemID.TitanGlove),
            new CommonChestRule(ItemID.PhilosophersStone),
            new CommonChestRule(ItemID.CrossNecklace),
            new CommonChestRule(ItemID.StarCloak),
        ]);
        ChestLootDatabase.Instance.Register(ChestPool.UndergroundHard, bombs);
        ChestLootDatabase.Instance.Register(ChestPool.UndergroundHard, angelStatue);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.UndergroundHard, bars);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.UndergroundHard, ammo);
        ChestLootDatabase.Instance.Register(ChestPool.UndergroundHard, healingPotion);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.UndergroundHard, damageBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.UndergroundHard, healthBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.UndergroundHard, utilBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.UndergroundHard, miningBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.UndergroundHard, torches);
        ChestLootDatabase.Instance.Register(ChestPool.UndergroundHard, money);
        #endregion

        #region Desert loot
        ChestLootDatabase.Instance.RegisterIndexed(1, ChestPool.DesertHard, [
            new CommonChestRule(ItemID.DualHook),
            new CommonChestRule(ItemID.MagicDagger),
            new CommonChestRule(ItemID.TitanGlove),
            new CommonChestRule(ItemID.PhilosophersStone),
            new CommonChestRule(ItemID.CrossNecklace),
            new CommonChestRule(ItemID.StarCloak),
        ]);
        ChestLootDatabase.Instance.Register(ChestPool.DesertHard, bombs);
        ChestLootDatabase.Instance.Register(ChestPool.DesertHard, angelStatue);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, bars);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, ammo);
        ChestLootDatabase.Instance.Register(ChestPool.DesertHard, healingPotion);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, damageBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, healthBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, utilBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, miningBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, torches);
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.DesertHard, ItemID.AncientBattleArmorMaterial, chanceDemoninator: 3);
        ChestLootDatabase.Instance.Register(ChestPool.DesertHard, money);
        #endregion

        #region Snow loot
        ChestLootDatabase.Instance.RegisterIndexed(1, ChestPool.SnowHard, [
            new CommonChestRule(ItemID.Frostbrand),
            new CommonChestRule(ItemID.IceBow),
            new CommonChestRule(ItemID.FlowerofFrost),
        ]);
        ChestLootDatabase.Instance.Register(ChestPool.SnowHard, bombs);
        ChestLootDatabase.Instance.Register(ChestPool.SnowHard, angelStatue);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.SnowHard, bars);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.SnowHard, ammo);
        ChestLootDatabase.Instance.Register(ChestPool.SnowHard, healingPotion);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.SnowHard, damageBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.SnowHard, healthBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.SnowHard, utilBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.SnowHard, miningBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.SnowHard, [
            new CommonChestRule(ItemID.IceTorch, MinStack: minTorch, MaxStack: maxTorch),
            new CommonChestRule(ItemID.Glowstick, MinStack: minTorch, MaxStack: maxTorch),
        ]);
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.SnowHard, ItemID.FrostCore, chanceDemoninator: 3);
        ChestLootDatabase.Instance.Register(ChestPool.SnowHard, money);
        #endregion

        #region Jungle loot
        ChestLootDatabase.Instance.RegisterIndexed(1, ChestPool.JungleHard, [
            new CommonChestRule(ModContent.ItemType<global::Aequus.Items.Weapons.Magic.Misc.Healer.SavingGrace>()),
            new CommonChestRule(ModContent.ItemType<global::Aequus.Items.Weapons.Melee.Swords.Nettlebane.Nettlebane>()),
            new CommonChestRule(ModContent.ItemType<global::Aequus.Items.Weapons.Ranged.Guns.Hitscanner.Hitscanner>()),
        ]);
        ChestLootDatabase.Instance.Register(ChestPool.JungleHard, bombs);
        ChestLootDatabase.Instance.Register(ChestPool.JungleHard, angelStatue);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.JungleHard, bars);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.JungleHard, ammo);
        ChestLootDatabase.Instance.Register(ChestPool.JungleHard, healingPotion);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.JungleHard, damageBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.JungleHard, healthBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.JungleHard, utilBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.JungleHard, miningBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.JungleHard, torches);
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.JungleHard, ItemID.TurtleShell, chanceDemoninator: 3);
        ChestLootDatabase.Instance.Register(ChestPool.JungleHard, money);
        #endregion

        SetStaticDefaults();
    }

    public override void Unload() {
        CountsAsChest.Clear();
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
        //if (tileID == TileID.Containers) {
        //    switch (chestStyle) {
        //        case ChestType.Gold:
        //        case ChestType.Marble:
        //        case ChestType.Granite:
        //        case ChestType.Mushroom:
        //            chest.item[0].SetDefaults(WorldGen.genRand.Next(HardmodeChestLoot));
        //            break;

        //        case ChestType.Frozen:
        //            chest.item[0].SetDefaults(WorldGen.genRand.Next(HardmodeSnowChestLoot));
        //            break;

        //        case ChestType.RichMahogany:
        //        case ChestType.Ivy:
        //            chest.item[0].SetDefaults(WorldGen.genRand.Next(HardmodeJungleChestLoot));
        //            break;
        //    }
        //}
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

                    int newChest = WorldGen.PlaceChest(x, y, notNearOtherChests: true, style: style);
                    if (Main.chest.IndexInRange(newChest)) {
                        continue;
                    }

                    k += 80;
                    if (style != -1 && Main.tile[Main.chest[newChest].x, Main.chest[newChest].y].TileType == chestType) {
                        var chestStyle = ChestType.GetStyle(Main.chest[newChest]);
                        if (chestStyle != style) {
                            for (int i = 0; i < 2; i++) {
                                for (int j = 0; j < 2; j++) {
                                    var tile = Main.tile[Main.chest[newChest].x + i, Main.chest[newChest].y + j];
                                    tile.TileFrameX = (short)(tile.TileFrameX % 36 + style * 36);
                                    tile.TileFrameY = (short)(tile.TileFrameY % 36 + style * 36);
                                }
                            }
                        }
                    }

                    ChestOpenedTracker.UnopenedChests.Add(new Point(Main.chest[newChest].x, Main.chest[newChest].y));
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
}