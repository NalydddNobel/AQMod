using Aequus.Common.Hooks;
using Aequus.Common.Tiles;
using Aequus.Content.Chests;
using Aequus.Content.Configuration;
using Aequus.Content.Items.Potions.Healing.Restoration;
using Aequus.DataSets;
using Aequus.DataSets.Structures.DropRulesChest;
using Aequus.DataSets.Structures.Enums;
using System;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Aequus.Old.Content.Chests;

public sealed class HardmodeChestSystem : ModSystem {
    public override bool IsLoadingEnabled(Mod mod) {
        return GameplayConfig.Instance.HardmodeChests;
    }

    public const byte TIER_COBALT = 1;
    public const byte TIER_MYTHRIL = 2;
    public const byte TIER_ADAMANTITE = 3;

    private static byte _chestTierOld;
    private static byte _chestTier;

    public readonly Action[] TierActions = new Action[3];

    private static void CobaltTierChests() {
        PlaceNewChests();
        HardmodifyChests();
        WorldGen.BroadcastText(NetworkText.FromKey("Mods.Aequus.Announcement.HardmodeChests"), CommonColor.TextEvent.HueAdd(0.075f));
    }

    private static void GetChestParams(int x, int y, out int style) {
        var r = new Rectangle(x - 5, y - 5, 10, 10);

        style = -1;
        if (TileHelper.ScanTiles(r, TileHelper.HasTileAction(TileID.MushroomGrass, TileID.MushroomBlock))) {
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
    }

    private static void PlaceNewChests() {
        for (int k = 0; k < Main.maxTilesX + Main.maxTilesY; k++) {
            int x = WorldGen.genRand.Next(100, Main.maxTilesX);
            int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer - 50);
            Tile tile = Main.tile[x, y];
            ushort wallType = tile.WallType;
            if (!Main.wallHouse[wallType] && !Main.wallDungeon[wallType] && wallType != WallID.LihzahrdBrickUnsafe && wallType != WallID.HiveUnsafe) {
                int chestType = TileID.Containers;
                GetChestParams(x, y, out int style);
                if (!WorldGen.AddBuriedChest(new Point(x, y), notNearOtherChests: true, Style: style)) {
                    continue;
                }

                for (int l = y - 8; l < Main.maxTilesY - 10; l++) {
                    int guess = Chest.FindChestByGuessing(x, l);
                    if (guess == -1) {
                        guess = Chest.FindChestByGuessing(x - 1, l);
                    }

                    if (guess != -1) {
                        k += 80;
                        if (style != -1 && Main.tile[Main.chest[guess].x, Main.chest[guess].y].TileType == chestType) {
                            for (int i = 0; i < 2; i++) {
                                for (int j = 0; j < 2; j++) {
                                    Tile chestTile = Main.tile[Main.chest[guess].x + i, Main.chest[guess].y + j];
                                    chestTile.TileFrameX = (short)(chestTile.TileFrameX % 36 + style * 36);
                                    chestTile.TileFrameY = (short)(chestTile.TileFrameY % 36 + style * 36);
                                }
                            }
                        }
                        UnopenedChestItem.Place(Main.chest[guess]);
                        break;
                    }
                }
            }
        }
    }

    private static void HardmodifyChests() {
        for (int i = 0; i < Main.maxChests; i++) {
            Chest chest = Main.chest[i];
            if (chest == null || !chest.IsUnopened() || chest.HasImportantItem()) {
                continue;
            }

            HardmodifySingleChest(i);

            if (Main.netMode != NetmodeID.SinglePlayer) {
                ChestConversionNetUpdate(i);
            }
        }
    }

    private static void HardmodifySingleChest(int i) {
        Chest chest = Main.chest[i];
        Tile tile = Main.tile[chest.x, chest.y];
        if (chest.bankChest || !tile.HasTile || !TileID.Sets.IsAContainer[tile.TileType]) {
            return;
        }

        ChestLootInfo info = new ChestLootInfo(i, WorldGen.genRand);
        ChestStyle style = ChestStyleConversion.ToEnum(tile.TileType, tile.TileFrameX / 36);

        if (!TileDataSet.ChestToBiome.TryGetValue(style, out Biome biome)) {
            return;
        }

        // Add loot
        switch (biome) {
            case Biome.Underground:
                ChestLootDatabase.Instance.SolveRules(ChestPool.HardmodeChestRegular, in info);
                break;
            case Biome.UndergroundSnow:
                ChestLootDatabase.Instance.SolveRules(ChestPool.HardmodeChestSnow, in info);
                break;
            case Biome.UndergroundJungle:
                ChestLootDatabase.Instance.SolveRules(ChestPool.HardmodeChestJungle, in info);
                break;
        }

        // Turn into hardmode sprite
        if (HardmodeChestLoader.StyleToChest.TryGetValue(style, out ModTile hardmodeChest)) {
            for (int k = 0; k < 2; k++) {
                for (int l = 0; l < 2; l++) {
                    Tile replaceTile = Main.tile[chest.x + k, chest.y + l];
                    replaceTile.TileType = hardmodeChest.Type;
                    replaceTile.TileFrameX = (short)(k * 18);
                    replaceTile.TileFrameY = (short)(l * 18);
                }
            }
        }
    }

    public static void ChestConversionNetUpdate(int chestId) {
        Chest chest = Main.chest[chestId];
        Tile tile = Main.tile[chest.x, chest.y];
        int weirdThing = 1;
        if (tile.TileType == TileID.Containers2) {
            weirdThing = 5;
        }
        if (tile.TileType >= TileID.Count) {
            weirdThing = 101;
        }
        NetMessage.SendData(MessageID.ChestUpdates, -1, -1, null, weirdThing, chest.x, chest.y, 0f, chestId, tile.TileType);
        NetMessage.SendTileSquare(-1, chest.x, chest.y, 3);
    }

    public static void OnRandomTileUpdate(int i, int j) {
        if (!Main.tile[i, j].HasTile) {
            return;
        }

        int type = Main.tile[i, j].TileType;
        if (WorldGen.SavedOreTiers.Cobalt > 0 && type == WorldGen.SavedOreTiers.Cobalt) {
            SetTier(TIER_COBALT);
        }
        if (WorldGen.SavedOreTiers.Mythril > 0 && type == WorldGen.SavedOreTiers.Mythril) {
            SetTier(TIER_MYTHRIL);
        }
        if (WorldGen.SavedOreTiers.Adamantite > 0 && type == WorldGen.SavedOreTiers.Adamantite) {
            SetTier(TIER_ADAMANTITE);
        }
    }

    public override void PostUpdateWorld() {
        //if (Main.rand.NextBool(240)) {
        //    _chestTier = 0;
        //    _chestTierOld = 0;
        //}
        if (!Main.hardMode || WorldGen.IsGeneratingHardMode || Main.netMode == NetmodeID.MultiplayerClient || _chestTier == _chestTierOld) {
            return;
        }

        int start = _chestTierOld;
        int end = Math.Min(_chestTier, TierActions.Length);
        for (int i = start; i < end; i++) {
            TierActions[i]?.Invoke();
        }
        _chestTierOld = _chestTier;
    }

    public override void Load() {
        TierActions[TIER_COBALT] += CobaltTierChests;
    }

    public override void SetStaticDefaults() {
        TerrariaHooks.OnRandomTileUpdate += OnRandomTileUpdate;
        ChestLootDatabase.Instance.Register(ChestPool.HardmodeChestRegular, new ReplaceFirstSlotChestRule(
            new IndexedChestRule(new IChestLootRule[] {
                    new CommonChestRule(ItemID.DualHook),
                    new CommonChestRule(ItemID.MagicDagger, OptionalConditions: Condition.NotRemixWorld),
                    new CommonChestRule(ItemID.WandofSparking, OptionalConditions: Condition.RemixWorld),
                    new CommonChestRule(ItemID.TitanGlove),
                    new CommonChestRule(ItemID.PhilosophersStone),
                    new CommonChestRule(ItemID.CrossNecklace),
                    new CommonChestRule(ItemID.StarCloak)
                }
            )
        ));
        ChestLootDatabase.Instance.Register(ChestPool.HardmodeChestSnow, new ReplaceFirstSlotChestRule(
            new IndexedChestRule(new IChestLootRule[] {
                    new CommonChestRule(ItemID.Frostbrand),
                    new CommonChestRule(ItemID.IceBow, OptionalConditions: Condition.NotRemixWorld),
                    new CommonChestRule(ItemID.SnowballCannon, OptionalConditions: Condition.RemixWorld),
                    new CommonChestRule(ItemID.FlowerofFrost)
                }
            )
        ));
        ChestLootDatabase.Instance.Register(ChestPool.HardmodeChestJungle, new ReplaceFirstSlotChestRule(
            new IndexedChestRule(new IChestLootRule[] {
                    new CommonChestRule(ItemID.Seedler),
                    new CommonChestRule(ItemID.VenusMagnum),
                    new CommonChestRule(ItemID.NettleBurst),
                }
            )
        ));

        int ammoMin = 50;
        int ammoMax = 100;

        // Replace Wooden and Flaming Arrows with Unholy Arrows.
        RegisterGenericHardmodeChestRule(new ReplaceMultipleItemsChestRule(new int[] {
            ItemID.WoodenArrow, ItemID.FlamingArrow
        }, new CommonChestRule(ItemID.UnholyArrow, MinStack: ammoMin, MaxStack: ammoMax)));

        // Replace Shurikens, Throwing Knives, and Flares with Silver/Tungsten bullets.
        Condition silverBulletCondition = new Condition("Mods.Aequus.Condition.SilverOre", () => WorldGen.SavedOreTiers.Silver == TileID.Silver);
        RegisterGenericHardmodeChestRule(new ReplaceMultipleItemsChestRule(new int[] {
            ItemID.Shuriken, ItemID.ThrowingKnife, ItemID.Flare
        }, new CommonChestRule(ItemID.SilverBullet, MinStack: ammoMin, MaxStack: ammoMax, OptionalConditions: silverBulletCondition)
        .OnFailure(new CommonChestRule(ItemID.TungstenBullet, MinStack: ammoMin, MaxStack: ammoMax))));

        // Replace Recalls with Return potions.
        RegisterGenericHardmodeChestRule(new ReplaceMultipleItemsChestRule(new int[] {
            ItemID.RecallPotion
        }, new CommonChestRule(ItemID.PotionOfReturn, MinStack: 1, MaxStack: 3)));

        // Replace Glowsticks with Spelunker Glowsticks.
        RegisterGenericHardmodeChestRule(new ReplaceMultipleItemsChestRule(new int[] {
            ItemID.Glowstick, ItemID.StickyGlowstick
        }, new CommonChestRule(ItemID.SpelunkerGlowstick, MinStack: 50, MaxStack: 150)));

        // Replace Healing Potions with Greater Healing Potions.
        RegisterGenericHardmodeChestRule(new ReplaceMultipleItemsChestRule(new int[] {
            ItemID.LesserHealingPotion, ItemID.HealingPotion
        }, new CommonChestRule(ItemID.GreaterHealingPotion, MinStack: 2, MaxStack: 6)));

        // Replace Restoration Potions with Greater Restoration Potions.
        RegisterGenericHardmodeChestRule(new ReplaceMultipleItemsChestRule(new int[] {
            ModContent.ItemType<LesserRestorationPotion>(), ItemID.RestorationPotion
        }, new CommonChestRule(ModContent.ItemType<GreaterRestorationPotion>(), MinStack: 2, MaxStack: 6)));

        // Replace PHM boss items with HM boss items.
        RegisterGenericHardmodeChestRule(new ReplaceMultipleItemsChestRule(new int[] {
            ItemID.SuspiciousLookingEye, ItemID.SlimeCrown
        }, new IndexedChestRule(new[] {
            new CommonChestRule(ItemID.MechanicalEye),
            new CommonChestRule(ItemID.MechanicalWorm),
            new CommonChestRule(ItemID.MechanicalSkull),
        })));
    }

    private static void RegisterGenericHardmodeChestRule(IChestLootRule rule) {
        ChestLootDatabase.Instance.Register(ChestPool.HardmodeChestRegular, rule);
        ChestLootDatabase.Instance.Register(ChestPool.HardmodeChestSnow, rule);
        ChestLootDatabase.Instance.Register(ChestPool.HardmodeChestJungle, rule);
    }

    public override void SaveWorldData(TagCompound tag) {
        if (_chestTier != 0) {
            tag["ChestTier"] = _chestTier;
        }
        if (_chestTierOld != _chestTier) {
            tag["ChestTierOld"] = _chestTierOld;
        }
    }

    public override void LoadWorldData(TagCompound tag) {
        if (tag.TryGet("ChestTier", out byte chestTier)) {
            _chestTier = chestTier;
        }
        if (tag.TryGet("ChestTierOld", out byte chestTierOld)) {
            _chestTierOld = chestTierOld;
        }
    }

    public override void ClearWorld() {
        _chestTier = 0;
        _chestTierOld = 0;
    }

    public static void SetTier(byte tier) {
        _chestTier = Math.Max(tier, _chestTier);
    }
}
