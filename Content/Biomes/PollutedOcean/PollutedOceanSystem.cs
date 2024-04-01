using Aequus.Common.Chests;
using Aequus.Content.Chests;
using Aequus.Content.Configuration;
using Aequus.Content.CrossMod.SplitSupport.Photography;
using Aequus.Content.Enemies.PollutedOcean.BlackJellyfish;
using Aequus.Content.Enemies.PollutedOcean.BreadOfCthulhu;
using Aequus.Content.Enemies.PollutedOcean.Conductor;
using Aequus.Content.Enemies.PollutedOcean.OilSlime;
using Aequus.Content.Enemies.PollutedOcean.Scavenger;
using Aequus.Content.Fishing;
using Aequus.Content.Fishing.Baits.BlackJellyfish;
using Aequus.Content.Potions.Healing.Restoration;
using Aequus.Content.Tiles.Furniture.Trash;
using Aequus.Content.Tiles.Statues;
using Aequus.Content.Tools.AnglerLamp;
using Aequus.Content.Weapons.Ranged.Darts.Ammo;
using Aequus.Content.Weapons.Ranged.Darts.StarPhish;
using Aequus.Old.Content.Potions.PotionCanteen;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanSystem : ModSystem {
    public static List<int> BiomeTiles { get; private set; } = new();

    public static int PollutedTileThreshold { get; set; } = 800;
    public static int PollutedTileMax { get; set; } = 300;
    public static int PollutedTileCount { get; set; }

    private static int? _music;
    public static int Music => _music ??= MusicLoader.GetMusicSlot("AequusMusic/Assets/Music/PollutedOcean");

    public override void SetStaticDefaults() {
        PopulateCrateDrops();
        PopulateChestDrops();
    }

    private static void PopulateCrateDrops() {
        if (VanillaChangesConfig.Instance.MoveMagicConch) {
            PhotographyLoader.EnvelopePollutedOcean.MainItemDrops.Add(ItemID.MagicConch);
        }
        PhotographyLoader.EnvelopePollutedOcean.MainItemDrops.Add(ModContent.ItemType<AnglerLamp>());
        PhotographyLoader.EnvelopePollutedOcean.MainItemDrops.Add(ModContent.ItemType<StarPhish>());
#if !DEBUG
        PhotographyLoader.EnvelopePollutedOcean.MainItemDrops.Add(ModContent.ItemType<PotionCanteen>());
#endif
    }

    private static void PopulateChestDrops() {
        ChestLootDatabase.Instance.RegisterIndexed(ChestLoot.PollutedOcean,
            new ChestRules.Common(ItemID.MagicConch, OptionalConditions: Aequus.ConditionConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveMagicConch))),
            new ChestRules.Common(ModContent.ItemType<AnglerLamp>()),
        #if !DEBUG
            new ChestRules.Common(ModContent.ItemType<PotionCanteen>()),
        #endif
            new ChestRules.Common(ModContent.ItemType<StarPhish>()).OnSucceed(new ChestRules.Common(ModContent.ItemType<PlasticDart>(), MinStack: 25, MaxStack: 50))
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ItemID.BombFish,
            minStack: 10, maxStack: 19,
            chanceDemoninator: 3
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ModContent.GetInstance<AncientAngelStatue>().ItemDrop.Type,
            chanceDemoninator: 5
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ItemID.Chain,
            minStack: 10, maxStack: 25,
            chanceDemoninator: 3
        );

        ChestLootDatabase.Instance.Register(ChestLoot.PollutedOcean, new ChestRules.OneFromOptions(new IChestLootRule[] {
                new ChestRules.MetalBar(() => WorldGen.SavedOreTiers.Copper, MinStack: 5, MaxStack: 14),
                new ChestRules.MetalBar(() => WorldGen.SavedOreTiers.Iron, MinStack: 5, MaxStack: 14)
            },
            ChanceDenominator: 2
        ));
        ChestLootDatabase.Instance.Register(ChestLoot.PollutedOcean, new ChestRules.OneFromOptions(new IChestLootRule[] {
                new ChestRules.MetalBar(() => WorldGen.SavedOreTiers.Silver, MinStack: 5, MaxStack: 14),
                new ChestRules.MetalBar(() => WorldGen.SavedOreTiers.Gold, MinStack: 5, MaxStack: 14)
            },
            ChanceDenominator: 3
        ));
        ChestLootDatabase.Instance.Register(ChestLoot.PollutedOcean, new ChestRules.OneFromOptions(new IChestLootRule[] {
                new ChestRules.Common(ItemID.BoneArrow, MinStack: 25, MaxStack: 49),
                new ChestRules.Common(ItemID.SpikyBall, MinStack: 25, MaxStack: 49)
            },
            ChanceDenominator: 2
        ));
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ModContent.ItemType<LesserRestorationPotion>(),
            minStack: 3, maxStack: 5,
            chanceDemoninator: 5
        );

        ChestLootDatabase.Instance.Register(ChestLoot.PollutedOcean, new ChestRules.OneFromOptions(new IChestLootRule[] {
                new ChestRules.Common(ItemID.GillsPotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.FlipperPotion, MinStack: 1, MaxStack: 2),
            },
            ChanceDenominator: 3, ChanceNumerator: 1
        ));
        ChestLootDatabase.Instance.Register(ChestLoot.PollutedOcean, new ChestRules.OneFromOptions(new IChestLootRule[] {
                new ChestRules.Common(ItemID.RegenerationPotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.ShinePotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.NightOwlPotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.SwiftnessPotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.ArcheryPotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.HunterPotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.MiningPotion, MinStack: 1, MaxStack: 2),
                new ChestRules.Common(ItemID.TrapsightPotion, MinStack: 1, MaxStack: 2),
            },
            ChanceDenominator: 3, ChanceNumerator: 2
        ));

        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ModContent.GetInstance<TrashTorch>().Item.Type,
            minStack: 10, maxStack: 20,
            chanceDemoninator: 2
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ItemID.RecallPotion,
            minStack: 1, maxStack: 2,
            chanceDemoninator: 3, chanceNumerator: 2
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ItemID.GoldCoin,
            chanceDemoninator: 2
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestLoot.PollutedOcean, ItemID.SilverCoin,
            minStack: 1, maxStack: 99
        );

        ChestLootDatabase.Instance.Register(ChestLoot.UndergroundDesert, new ChestRules.Replace(
            ItemIdToReplace: ItemID.MagicConch,
            new ChestRules.Indexed(new IChestLootRule[] {
                    new ChestRules.Common(ItemID.SandstorminaBottle),
                    new ChestRules.Common(ItemID.FlyingCarpet)
                }
            ),
            Aequus.ConditionConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveMagicConch))
        ));
    }

    public override void Unload() {
        BiomeTiles.Clear();
        _music = null;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        PollutedTileCount = 0;
        foreach (var tile in BiomeTiles) {
            PollutedTileCount += tileCounts[tile];
        }
    }

    public static bool CheckBiome(Player player) {
        return PollutedTileCount >= PollutedTileMax;
    }

    public static void PopulateSurfaceSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool[0] *= 0.5f;

        pool[ModContent.NPCType<OilSlime>()] = 1f;

        if (spawnInfo.Water) {
            pool[NPCID.Arapaima] = 1f; // Eel
            pool[NPCID.Tumbleweed] = 1f; // Urchin

            pool[NPCID.LightningBug] = 0.1f; // Sea Firefly
        }

        if (Main.hardMode) {
            pool[NPCID.AngryNimbus] = 0.33f; // Mirage
        }
    }

    public static void PopulateUndergroundSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool.Clear();

        pool[ModContent.NPCType<OilSlime>()] = 1f;
        pool[ModContent.NPCType<Scavenger>()] = 0.8f;
        pool[ModContent.NPCType<Conductor>()] = 0.1f;

        if (spawnInfo.Water) {
            pool[ModContent.NPCType<BlackJellyfish>()] = 1f;
            pool[NPCID.Arapaima] = 1f; // Eel
            pool[NPCID.Tumbleweed] = 1f; // Urchin

            pool[NPCID.LightningBug] = 0.1f; // Sea Firefly
        }

        if (Main.hardMode) {
            pool[NPCID.MushiLadybug] = 0.33f; // Pillbug
        }

        pool[NPCID.Buggy] = 0.1f; // Chromite
        pool[NPCID.Sluggy] = 0.1f; // Horseshoe Crab
    }

    public static void CatchSurfaceFish(in FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) {
        CatchCommonFish(in attempt, ref itemDrop, ref npcSpawn);
    }

    public static void CatchUndergroundFish(in FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) {
        if (attempt.rare && Main.rand.NextBool(5)) {
            itemDrop = ModContent.ItemType<BlackJellyfishBait>();
            return;
        }

        CatchCommonFish(in attempt, ref itemDrop, ref npcSpawn);
    }

    private static void CatchCommonFish(in FishingAttempt attempt, ref int itemDrop, ref int npcSpawn) {
        if (attempt.common && Main.rand.NextBool()) {
            if (Main.rand.NextBool()) {
                itemDrop = FishInstantiator.Killifish.Type;
            }
            else {
                itemDrop = FishInstantiator.Piraiba.Type;
            }
        }

        int chance = BreadOfCthulhu.GetFishingChance(in attempt);
        if (Main.rand.NextBool(chance)) {
            npcSpawn = ModContent.NPCType<BreadOfCthulhu>();
        }
    }
}