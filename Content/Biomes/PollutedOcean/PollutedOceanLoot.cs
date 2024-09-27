#if POLLUTED_OCEAN
using Aequus.Common;
using Aequus.Common.Preferences;
using Aequus.Content.Biomes.PollutedOcean.Generation;
using Aequus.Content.Fishing.Crates;
using Aequus.Content.Items.Materials.Ores;
using Aequus.Systems.Chests;
using Aequus.Systems.Chests.DropRules;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Biomes.PollutedOcean;

public class PollutedOceanLoot : LoadedType {
    protected override void PostSetupContent() {
        #region Crate Drops
        List<IItemDropRule> crateDrops = [];
        if (GameplayConfig.Instance.MoveMagicConch) {
            crateDrops.Add(ItemDropRule.NotScalingWithLuck(ItemID.MagicConch));
        }
#if POLLUTED_OCEAN_TODO
        crateDrops.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Items.Tools.AnglerLamp.AnglerLamp>()));
#endif

        IItemDropRule starphishRule = ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Items.Weapons.Ranged.StarPhish.StarPhish>());
#if POLLUTED_OCEAN_TODO
        starphishRule.OnSuccess(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Items.Weapons.Ranged.Ammo.PlasticDart>(), minimumDropped: 25, maximumDropped: 50));
#endif
        crateDrops.Add(starphishRule);

        foreach (IItemDropRule rule in crateDrops) {
            Instance<PollutedOceanCrate>().Primary.Add(rule);
#if SPLIT
            PhotographyLoader.EnvelopePollutedOcean.Primary.Add(rule);
#endif
        }
        #endregion

        IChestLootRule[] primaryChestRules = [
#if POLLUTED_OCEAN_TODO
            new CommonChestRule(ModContent.ItemType<Items.Tools.AnglerLamp.AnglerLamp>()),
#endif
            new CommonChestRule(ModContent.ItemType<Items.Accessories.PotionCanteen.PotionCanteen>()),
            new CommonChestRule(ModContent.ItemType<Items.Weapons.Ranged.StarPhish.StarPhish>())
#if POLLUTED_OCEAN_TODO
            .OnSucceed(new CommonChestRule(ModContent.ItemType<Items.Weapons.Ranged.Ammo.PlasticDart>(), MinStack: 25, MaxStack: 50))
#endif
        ];
        if (GameplayConfig.Instance.MoveMagicConch) {
            Helper.Add(ref primaryChestRules, new CommonChestRule(ItemID.MagicConch));
        }

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.BombFish,
            minStack: 3, maxStack: 7,
            chanceDenominator: 4
        );
#if POLLUTED_OCEAN_TODO
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.GetInstance<AncientAngelStatue>().ItemDrop.Type,
            chanceDemoninator: 10
        );
#endif
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.Chain,
            minStack: 2, maxStack: 5,
            chanceDenominator: 3
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, Instance<Aluminum>().BarItem.Type,
            minStack: 2,
            maxStack: 6,
            chanceDenominator: 3
        );

        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new OneFromOptionsChestRule([
                new CommonChestRule(ItemID.BoneArrow, MinStack: 9, MaxStack: 24),
                new CommonChestRule(ItemID.SpikyBall, MinStack: 9, MaxStack: 24)
            ],
            ChanceDenominator: 3
        ));
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.ItemType<global::Aequus.Items.Potions.Healing.Restoration.LesserRestorationPotion>(),
            minStack: 1, maxStack: 3,
            chanceDenominator: 3
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

#if POLLUTED_OCEAN_TODO
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ModContent.GetInstance<SeaPickleTorch>().Item.Type,
            minStack: 5, maxStack: 10,
            chanceDemoninator: 2
        );
#else
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.CoralTorch,
            minStack: 5, maxStack: 10,
            chanceDenominator: 2
        );
#endif

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.RecallPotion,
            minStack: 1, maxStack: 2,
            chanceDenominator: 8, chanceNumerator: 1
        );

        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.GoldCoin,
            chanceDenominator: 3
        );
        ChestLootDatabase.Instance.RegisterCommon(ChestPool.PollutedOcean, ItemID.SilverCoin,
            minStack: 1, maxStack: 99
        );

        // Random Trash
        int[] trashItems = [ModContent.ItemType<Items.Materials.CompressedTrash.CompressedTrash>(), ItemID.FishingSeaweed, ItemID.TinCan, ItemID.OldShoe];
        ChestLootDatabase.Instance.Register(ChestPool.PollutedOcean, new RandomTrashChestRule(trashItems, 1, 4, 0, 9));

        if (GameplayConfig.Instance.MoveMagicConch) {
            ChestLootDatabase.Instance.Register(ChestPool.UndergroundDesert, new ReplaceItemChestRule(
                ItemIdToReplace: ItemID.MagicConch,
                new IndexedChestRule(1, [
                        new CommonChestRule(ItemID.SandstorminaBottle),
                    new CommonChestRule(ItemID.FlyingCarpet)
                    ]
                )
            ));
        }
    }
}
#endif