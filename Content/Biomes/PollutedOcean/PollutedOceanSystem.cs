using Aequu2.Content.Configuration;
using Aequu2.Content.Critters.SeaFirefly;
using Aequu2.Content.CrossMod.SplitSupport.Photography;
using Aequu2.Content.Enemies.PollutedOcean.BlackJellyfish;
using Aequu2.Content.Enemies.PollutedOcean.BreadOfCthulhu;
using Aequu2.Content.Enemies.PollutedOcean.Conductor;
using Aequu2.Content.Enemies.PollutedOcean.Conehead;
using Aequu2.Content.Enemies.PollutedOcean.Eel;
using Aequu2.Content.Enemies.PollutedOcean.OilSlime;
using Aequu2.Content.Enemies.PollutedOcean.Scavenger;
using Aequu2.Content.Fishing;
using Aequu2.Content.Fishing.Baits.BlackJellyfish;
using Aequu2.Content.Items.Potions.Healing.Restoration;
using Aequu2.Content.Items.Potions.PotionCanteen;
using Aequu2.Content.Items.Tools.AnglerLamp;
using Aequu2.Content.Items.Weapons.Ranged.Ammo;
using Aequu2.Content.Items.Weapons.Ranged.StarPhish;
using Aequu2.Content.Tiles.Furniture.Trash;
using Aequu2.Content.Tiles.Statues;
using Aequu2.DataSets;
using Aequu2.DataSets.Structures.DropRulesChest;
using Aequu2.DataSets.Structures.Enums;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace Aequu2.Content.Biomes.PollutedOcean;

public class PollutedOceanSystem : ModSystem {
    public static int PollutedTileThreshold { get; set; } = 800;
    public static int PollutedTileMax { get; set; } = 300;
    public static int PollutedTileCount { get; set; }

    private static int? _music;
    public static int Music => _music ??= MusicLoader.GetMusicSlot("Aequu2Music/Assets/Music/PollutedOcean");

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
        PhotographyLoader.EnvelopePollutedOcean.MainItemDrops.Add(ModContent.ItemType<PotionCanteen>());
    }

    private static void PopulateChestDrops() {
        ChestLootDatabase.Instance.RegisterIndexed(ChestPool.PollutedOcean,
            new CommonChestRule(ItemID.MagicConch, OptionalConditions: Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveMagicConch))),
            new CommonChestRule(ModContent.ItemType<AnglerLamp>()),
            new CommonChestRule(ModContent.ItemType<PotionCanteen>()),
            new CommonChestRule(ModContent.ItemType<StarPhish>()).OnSucceed(new CommonChestRule(ModContent.ItemType<PlasticDart>(), MinStack: 25, MaxStack: 50))
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.BombFish,
            minStack: 10, maxStack: 19,
            chanceDemoninator: 3
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.GetInstance<AncientAngelStatue>().ItemDrop.Type,
            chanceDemoninator: 5
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.Chain,
            minStack: 10, maxStack: 25,
            chanceDemoninator: 3
        );

        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule(new IChestLootRule[] {
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Copper, MinStack: 5, MaxStack: 14),
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Iron, MinStack: 5, MaxStack: 14)
            },
            ChanceDenominator: 2
        ));
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule(new IChestLootRule[] {
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Silver, MinStack: 5, MaxStack: 14),
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Gold, MinStack: 5, MaxStack: 14)
            },
            ChanceDenominator: 3
        ));
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule(new IChestLootRule[] {
                new CommonChestRule(ItemID.BoneArrow, MinStack: 25, MaxStack: 49),
                new CommonChestRule(ItemID.SpikyBall, MinStack: 25, MaxStack: 49)
            },
            ChanceDenominator: 2
        ));
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.ItemType<LesserRestorationPotion>(),
            minStack: 3, maxStack: 5,
            chanceDemoninator: 5
        );

        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule(new IChestLootRule[] {
                new CommonChestRule(ItemID.GillsPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.FlipperPotion, MinStack: 1, MaxStack: 2),
            },
            ChanceDenominator: 3, ChanceNumerator: 1
        ));
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule(new IChestLootRule[] {
                new CommonChestRule(ItemID.RegenerationPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.ShinePotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.NightOwlPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.SwiftnessPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.ArcheryPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.HunterPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.MiningPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.TrapsightPotion, MinStack: 1, MaxStack: 2),
            },
            ChanceDenominator: 3, ChanceNumerator: 2
        ));

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.GetInstance<SeaPickleTorch>().Item.Type,
            minStack: 10, maxStack: 20,
            chanceDemoninator: 2
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.RecallPotion,
            minStack: 1, maxStack: 2,
            chanceDemoninator: 3, chanceNumerator: 2
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.GoldCoin,
            chanceDemoninator: 2
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.SilverCoin,
            minStack: 1, maxStack: 99
        );

        ChestLootDatabase.Instance.Register(ChestPool.UndergroundDesert, new ReplaceItemChestRule(
            ItemIdToReplace: ItemID.MagicConch,
            new IndexedChestRule(new IChestLootRule[] {
                    new CommonChestRule(ItemID.SandstorminaBottle),
                    new CommonChestRule(ItemID.FlyingCarpet)
                }
            ),
            Commons.Conditions.ConfigIsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveMagicConch))
        ));
    }

    public override void Unload() {
        _music = null;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        PollutedTileCount = 0;
        for (int i = 0; i < TileDataSet.GivesPollutedBiomePresence.Count; i++) {
            PollutedTileCount += tileCounts[TileDataSet.GivesPollutedBiomePresence[i]];
        }
    }

    public static bool CheckBiome(Player player) {
        return PollutedTileCount >= PollutedTileMax;
    }

    public static void PopulateSurfaceSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
        pool[0] *= 0.25f;

        if (Main.dayTime) {
            pool[ModContent.NPCType<OilSlime>()] = 1f;
        }
        else {
            List<ModNPC> zombies = ModContent.GetInstance<ConeheadZombieLoader>().Types;
            for (int i = 0; i < zombies.Count; i++) {
                pool[zombies[i].Type] = 1f / zombies.Count;
            }
        }

        if (spawnInfo.Water) {
            pool[ModContent.NPCType<Eel>()] = 0.1f;
            pool[NPCID.Tumbleweed] = 1f; // Urchin

            if (!Main.dayTime) {
                if (NPC.CountNPCS(ModContent.NPCType<SeaFirefly>()) < 25) {
                    pool[ModContent.NPCType<SeaFirefly>()] = 20f;
                }
            }
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
            pool[ModContent.NPCType<Eel>()] = 0.1f;
            pool[NPCID.Tumbleweed] = 1f; // Urchin

            if (NPC.CountNPCS(ModContent.NPCType<SeaFirefly>()) < 3) {
                pool[ModContent.NPCType<SeaFirefly>()] = 1f;
            }
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