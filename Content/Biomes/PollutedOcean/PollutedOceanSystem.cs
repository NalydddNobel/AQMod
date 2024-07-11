using AequusRemake.Content.Biomes.PollutedOcean.Generation;
using AequusRemake.Content.Configuration;
using AequusRemake.Content.Critters.SeaFirefly;
using AequusRemake.Content.Enemies.PollutedOcean.BlackJellyfish;
using AequusRemake.Content.Enemies.PollutedOcean.Conductor;
using AequusRemake.Content.Enemies.PollutedOcean.Conehead;
using AequusRemake.Content.Enemies.PollutedOcean.Eel;
using AequusRemake.Content.Enemies.PollutedOcean.OilSlime;
using AequusRemake.Content.Enemies.PollutedOcean.Scavenger;
using AequusRemake.Content.Fishing;
using AequusRemake.Content.Items.Materials;
using AequusRemake.Content.Tiles.Furniture.Trash;
using AequusRemake.Content.Tiles.Statues;
using AequusRemake.Core.Structures.Conditions;
using AequusRemake.DataSets;
using AequusRemake.Systems.Chests;
using AequusRemake.Systems.Chests.DropRules;
using AequusRemake.Systems.Configuration;
using AequusRemake.Systems.CrossMod.SplitSupport.Photography;
using AequusRemake.Systems.Fishing;
using AequusRemake.Systems.Fishing.Crates;
using System;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace AequusRemake.Content.Biomes.PollutedOcean;

public class PollutedOceanSystem : ModSystem {
    public int PollutedTileThreshold { get; set; } = 800;
    public int PollutedTileMax { get; set; } = 300;
    public int PollutedTileCount { get; set; }

    private int? _music;
    public int Music => _music ??= MusicLoader.GetMusicSlot("AequusRemakeMusic/Assets/Music/PollutedOcean");

    public ModItem Killifish { get; private set; }
    public ModItem Piraiba { get; private set; }

    public readonly Condition InSurfacePollutedOcean = ACondition.New("InSurfacePollutedOcean", () => Main.LocalPlayer.InModBiome<PollutedOceanBiomeSurface>());
    public readonly Condition InUndergroundPollutedOcean = ACondition.New("InUndergroundPollutedOcean", () => Main.LocalPlayer.InModBiome<PollutedOceanBiomeUnderground>());
    public readonly Condition InPollutedOcean = ACondition.New("InPollutedOcean", () => Main.LocalPlayer.InModBiome<PollutedOceanBiomeSurface>() || Main.LocalPlayer.InModBiome<PollutedOceanBiomeUnderground>());

    public override void Load() {
        AddFish();
    }

    public override void SetStaticDefaults() {
        PopulateDrops();
    }

    private void AddFish() {
        Killifish = new InstancedFishItem("Killifish", AequusTextures.Killifish.FullPath, ItemRarityID.Blue, Item.silver * 15, InstancedFishItem.SeafoodDinnerRecipe);
        Piraiba = new InstancedFishItem("Piraiba", AequusTextures.Piraiba.FullPath, ItemRarityID.Blue, Item.silver * 15, null);

        Mod.AddContent(Killifish);
        Mod.AddContent(Piraiba);

        FishLootDatabase.Instance.Add(new FishItemDropRule(Killifish.Type, ChanceDenominator: 3, Condition: InPollutedOcean), CatchTier.Common);
        FishLootDatabase.Instance.Add(new FishItemDropRule(Piraiba.Type, ChanceDenominator: 3, Condition: InPollutedOcean), CatchTier.Common);
    }

    private static void PopulateDrops() {
        #region Crate Drops
        List<IItemDropRule> crateDrops = [];
        if (VanillaChangesConfig.Instance.MoveMagicConch) {
            crateDrops.Add(ItemDropRule.NotScalingWithLuck(ItemID.MagicConch));
        }
        crateDrops.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Items.Tools.AnglerLamp.AnglerLamp>()));

        IItemDropRule starphishRule = ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Items.Weapons.Ranged.StarPhish.StarPhish>());
        starphishRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Items.Weapons.Ranged.Ammo.PlasticDart>(), minimumDropped: 25, maximumDropped: 50));
        crateDrops.Add(starphishRule);

        foreach (IItemDropRule rule in crateDrops) {
            ModContent.GetInstance<PollutedOceanCrate>().Primary.Add(rule);
            PhotographyLoader.EnvelopePollutedOcean.Primary.Add(rule);
        }
        #endregion

        ChestLootDatabase.Instance.RegisterIndexed(chanceDenominator: 2, ChestPool.PollutedOcean,
            new CommonChestRule(ItemID.MagicConch, OptionalConditions: ConfigConditions.IsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveMagicConch))),
            new CommonChestRule(ModContent.ItemType<Items.Tools.AnglerLamp.AnglerLamp>()),
            new CommonChestRule(ModContent.ItemType<Items.Potions.PotionCanteen.PotionCanteen>()),
            new CommonChestRule(ModContent.ItemType<Items.Weapons.Ranged.StarPhish.StarPhish>()).OnSucceed(new CommonChestRule(ModContent.ItemType<Items.Weapons.Ranged.Ammo.PlasticDart>(), MinStack: 25, MaxStack: 50))
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.BombFish,
            minStack: 3, maxStack: 7,
            chanceDemoninator: 4
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.GetInstance<AncientAngelStatue>().ItemDrop.Type,
            chanceDemoninator: 10
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.Chain,
            minStack: 2, maxStack: 5,
            chanceDemoninator: 3
        );

        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule([
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Copper, MinStack: 2, MaxStack: 6),
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Iron, MinStack: 2, MaxStack: 6)
            ],
            ChanceDenominator: 3
        ));
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule([
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Silver, MinStack: 2, MaxStack: 6),
                new MetalBarHackChestRule(() => WorldGen.SavedOreTiers.Gold, MinStack: 2, MaxStack: 6)
            ],
            ChanceDenominator: 3
        ));
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule([
                new CommonChestRule(ItemID.BoneArrow, MinStack: 9, MaxStack: 24),
                new CommonChestRule(ItemID.SpikyBall, MinStack: 9, MaxStack: 24)
            ],
            ChanceDenominator: 3
        ));
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.GetInstance<Items.Potions.RestorationPotions>().Lesser.Type,
            minStack: 1, maxStack: 3,
            chanceDemoninator: 3
        );

        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule([
                new CommonChestRule(ItemID.GillsPotion, MinStack: 1, MaxStack: 2),
                new CommonChestRule(ItemID.FlipperPotion, MinStack: 1, MaxStack: 2),
            ],
            ChanceDenominator: 3, ChanceNumerator: 1
        ));
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule([
                new CommonChestRule(ItemID.RegenerationPotion),
                new CommonChestRule(ItemID.ShinePotion),
                new CommonChestRule(ItemID.NightOwlPotion),
                new CommonChestRule(ItemID.SwiftnessPotion),
                new CommonChestRule(ItemID.ArcheryPotion),
                new CommonChestRule(ItemID.HunterPotion),
                new CommonChestRule(ItemID.MiningPotion),
                new CommonChestRule(ItemID.TrapsightPotion),
            ],
            ChanceDenominator: 3, ChanceNumerator: 1
        ));

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.GetInstance<SeaPickleTorch>().Item.Type,
            minStack: 5, maxStack: 10,
            chanceDemoninator: 2
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.RecallPotion,
            minStack: 1, maxStack: 2,
            chanceDemoninator: 8, chanceNumerator: 1
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.GoldCoin,
            chanceDemoninator: 3
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.SilverCoin,
            minStack: 1, maxStack: 99
        );

        // Random Trash
        int[] trashItems = [ModContent.ItemType<CompressedTrash>(), ItemID.FishingSeaweed, ItemID.TinCan, ItemID.OldShoe];
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new RandomTrashChestRule(trashItems, 1, 4, 0, 9));

        ChestLootDatabase.Instance.Register(ChestPool.UndergroundDesert, new ReplaceItemChestRule(
            ItemIdToReplace: ItemID.MagicConch,
            new IndexedChestRule(1, [
                    new CommonChestRule(ItemID.SandstorminaBottle),
                    new CommonChestRule(ItemID.FlyingCarpet)
                ]
            ),
            ConfigConditions.IsTrue(VanillaChangesConfig.Instance, nameof(VanillaChangesConfig.MoveMagicConch))
        ));

    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
        PollutedTileCount = 0;
        for (int i = 0; i < TileDataSet.GivesPollutedBiomePresence.Count; i++) {
            PollutedTileCount += tileCounts[TileDataSet.GivesPollutedBiomePresence[i]];
        }
    }

    public bool CheckBiome(Player player) {
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
}