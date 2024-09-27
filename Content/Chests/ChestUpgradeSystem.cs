using Aequus.Common.DataSets;
using Aequus.Common.Preferences;
using Aequus.Common.Tiles;
using Aequus.CrossMod;
using Aequus.Systems.Chests;
using Aequus.Systems.Chests.DropRules;
using System;
using Terraria.ModLoader.IO;

namespace Aequus.Content.Chests;

public sealed class ChestUpgradeSystem : ModSystem {
    public static ChestUpgradeSystem Instance => ModContent.GetInstance<ChestUpgradeSystem>();

    public const string Tag_Hardmode_Upgrade = "Upgrade";
    public const string Tag_Message = "Message";
    public const string Tag_New_Chests_Count = "Generate";

    public static readonly int NewChestsAreaDenominator = 32000;
    public static readonly int AlreadyInHardmodeNewChestsAreaDenominator = 240000;

    public bool HardmodeUpgradeMessage { get; set; }

    const int NewChestsSearchPerTickIterationCap = 100;
    int _newChestsWanted;

    int _nextUpgrade;

    public override void ClearWorld() {
        _nextUpgrade = -1;
        _newChestsWanted = 0;
        HardmodeUpgradeMessage = false;
    }

    public override void SaveWorldData(TagCompound tag) {
        if (HardmodeUpgradeMessage) {
            tag[Tag_Message] = HardmodeUpgradeMessage;
        }
        if (_newChestsWanted > 0) {
            tag[Tag_New_Chests_Count] = _newChestsWanted;
        }
    }

    public override void LoadWorldData(TagCompound tag) {
        _nextUpgrade = tag.GetOrDefault(Tag_Hardmode_Upgrade, -1);
        _newChestsWanted = tag.GetOrDefault(Tag_New_Chests_Count, 0);
        HardmodeUpgradeMessage = tag.GetOrDefault(Tag_Message, false);
    }

    internal void OnHardmodeBossDefeat() {
        _nextUpgrade = 0;
        if (!Main.hardMode) {
            _newChestsWanted = Math.Max(Main.maxTilesX * Main.maxTilesY / NewChestsAreaDenominator, 30);
        }
        else {
            _newChestsWanted = Math.Max(Main.maxTilesX * Main.maxTilesY / AlreadyInHardmodeNewChestsAreaDenominator, 5);
        }
    }

    public override void PostUpdateWorld() {
        if (!Main.hardMode || WorldGen.IsGeneratingHardMode || Main.netMode == NetmodeID.MultiplayerClient || !GameplayConfig.Instance.HardmodeChests) {
            return;
        }

        // First stage, we place new chests into the world.
        if (_newChestsWanted > 0) {
            Main.NewText(_newChestsWanted);
            for (int k = 0; k < NewChestsSearchPerTickIterationCap; k++) {
                if (TryPlaceNewChest()) {
                    _newChestsWanted--;
                    return;
                }
            }

            if (WorldGen.genRand.NextBool(100)) {
                _newChestsWanted--;
            }

            return;
        }

        // Second stage, we upgrade chests in the world.
        if (_nextUpgrade > -1 && _nextUpgrade < Main.maxChests) {

            for (int i = 0; i < 40 && _nextUpgrade < Main.maxChests; i++) {
                int next = _nextUpgrade++;

                Chest c = Main.chest[next];

                if (c == null || !c.IsUnopened() || c.FindItem(item => item != null && !item.IsAir && ItemSets.ImportantItem.Contains(item.type)) != null) {
                    continue;
                }

                Upgrade(c, next);
            }

            return;
        }


        // Finally, we display the upgrade message if we haven't already.
        if (!HardmodeUpgradeMessage) {
            HardmodeUpgradeMessage = true;
            TextHelper.Broadcast("Announcement.HardmodeChests", TextHelper.EventMessageColor.HueAdd(0.075f));
        }
    }

    bool TryPlaceNewChest() {
        int x = WorldGen.genRand.Next(100, Main.maxTilesX);
        int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer - 50);

        if (Main.wallHouse[Main.tile[x, y].WallType] || Main.tile[x, y].IsDungeonOrTempleWall() || Main.tile[x, y].WallType == WallID.HiveUnsafe || Main.tile[x, y].WallType >= WallID.Count) {
            return false;
        }

        var r = new Rectangle(x - 5, y - 5, 10, 10);
        TileKey chest = ChestID.Gold;
        foreach (var env in ChestSets.Instance.ChestEnvironment) {
            if (TileHelper.ScanTiles(r, env.ValidEnvionrment)) {
                chest = env.Chest;
                break;
            }
        }

        int newChest = WorldGen.PlaceChest(x, y, notNearOtherChests: true, type: chest.Type, style: chest.Style);
        if (!Main.chest.IndexInRange(newChest)) {
            return false;
        }

        Chest c = Main.chest[newChest];

        c.item[^1].SetDefaults(ModContent.ItemType<UnopenedChestTracker>());

        Instance<MagicChestPlacementEffect>().NewEffect(c.x, c.y);

        return true;
    }

    internal void Upgrade(Chest chest, int i) {
        int type = Main.tile[chest.x, chest.y].TileType;
        int style = ChestType.GetStyle(chest);

        if (!ChestSets.Instance.HardmodeConvert.TryGetValue((type, style), out var info)) {
            return;
        }

        // Clear chest of vanilla and Aequus items.
        // Other modded items are not deleted.
        ClearChest(chest);

        ChestLootDatabase.Instance.SolveRules(info.Loot, new ChestLootInfo(i, WorldGen.genRand));

        // Change to hardmode variant.
        ChestTools.Convert(chest.x, chest.y, info.ConvertType.Type, info.ConvertType.Style);

        // Stack duplicate item types together.
        ChestTools.StackDuplicateContents(chest);

        // Move all items to their lowest possible slot.
        ChestTools.MoveItemsToLowestUnoccupiedSlot(chest);

        if (Main.netMode != NetmodeID.SinglePlayer) {
            ChestTools.NetUpdate(i);
        }

        Instance<MagicChestPlacementEffect>().NewEffect(chest.x, chest.y);
    }

    void ClearChest(Chest chest) {
        for (int k = 0; k < chest.item.Length; k++) {
            Item item = (chest.item[k] ??= new());
            if (item.ModItem == null || item.ModItem.Mod is Aequus) {
                item.TurnToAir();
            }
        }
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
        int minBuff = 1;
        int maxBuff = 4;
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
        ChestLootDatabase.Instance.Register(ChestPool.DesertHard, new CommonChestRule(ModContent.ItemType<Items.Tools.Consumable.ScarabDynamite.ScarabDynamite>(), minBombs, maxBombs, ChanceDenominator: 3));
        ChestLootDatabase.Instance.Register(ChestPool.DesertHard, angelStatue);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, bars);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, ammo);
        ChestLootDatabase.Instance.Register(ChestPool.DesertHard, healingPotion);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, damageBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, healthBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, utilBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, miningBuffPotions);
        ChestLootDatabase.Instance.RegisterOneFromOptions(2, ChestPool.DesertHard, torches);
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.DesertHard, ItemID.AncientBattleArmorMaterial, chanceDenominator: 3);
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
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.SnowHard, ItemID.FrostCore, chanceDenominator: 3);
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
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.JungleHard, ItemID.TurtleShell, chanceDenominator: 3);
        ChestLootDatabase.Instance.Register(ChestPool.JungleHard, money);
        #endregion

        SetStaticDefaults();
    }
}