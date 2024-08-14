using Aequus.Common.DataSets;
using Aequus.Common.Preferences;
using Aequus.Common.Utilities;
using Aequus.Content.CursorDyes.Items;
using Aequus.Items.Equipment.Accessories.Misc;
using Aequus.Items.Equipment.PetsUtility.Miner;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Misc.FishingBait;
using Aequus.Items.Weapons.Melee.Swords.IronLotus;
using Aequus.Systems.Chests;
using Aequus.Systems.Chests.DropRules;
using Terraria.GameContent.ItemDropRules;

namespace Aequus;
public partial class AequusItem {
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (LootBuilder.registerToItem.TryGetValue(item.type, out var list) && list != null) {
            foreach (var rule in list) {
                itemLoot.Add(rule);
            }
        }

        switch (item.type) {
            case ItemID.EyeOfCthulhuBossBag: {
                    if (!GameplayConfig.Instance.EyeOfCthulhuOres) {
                        break;
                    }
                    itemLoot.RemoveWhere<ItemDropWithConditionRule>(r => r.itemId == ItemID.DemoniteOre);
                    itemLoot.RemoveWhere<ItemDropWithConditionRule>(r => r.itemId == ItemID.CrimtaneOre);
                }
                break;

            case ItemID.BossBagBetsy: {
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<IronLotus>()));
                }
                break;

            case ItemID.MoonLordBossBag:
                if (GameplayConfig.Instance.EarlyGravityGlobe)
                    itemLoot.RemoveWhere((itemDrop) => itemDrop is CommonDrop commonDrop && commonDrop.itemId == ItemID.GravityGlobe);
                if (GameplayConfig.Instance.EarlyPortalGun)
                    itemLoot.RemoveWhere((itemDrop) => itemDrop is CommonDrop commonDrop && commonDrop.itemId == ItemID.PortalGun);
                break;

            case ItemID.PlanteraBossBag:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                break;

            // Golden and Titanium Crate loot
            case ItemID.GoldenCrateHard:
                foreach (IChestLootRule rule in ChestLootDatabase.Instance.GetRulesForType(ChestPool.UndergroundHard)) {
                    if (rule is IndexedChestRule indexedRule) {
                        itemLoot.Add(indexedRule.ToItemDropRule());
                    }
                }
                goto case ItemID.GoldenCrate;

            case ItemID.GoldenCrate:
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SwordCursor>(), 4));
                break;

            // Iron and Mythril Crate Loot
            case ItemID.IronCrate:
            case ItemID.IronCrateHard:
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GlowCore>(), 6));
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MiningPetSpawner>(), 6));
                break;

            // Jungle and Bramble Crate loot
            case ItemID.JungleFishingCrateHard:
                foreach (IChestLootRule rule in ChestLootDatabase.Instance.GetRulesForType(ChestPool.JungleHard)) {
                    if (rule is IndexedChestRule indexedRule) {
                        itemLoot.Add(indexedRule.ToItemDropRule());
                    }
                }
                goto case ItemID.JungleFishingCrate;

            // Frozen and Boreal Crate loot
            case ItemID.FrozenCrateHard:
                foreach (IChestLootRule rule in ChestLootDatabase.Instance.GetRulesForType(ChestPool.SnowHard)) {
                    if (rule is IndexedChestRule indexedRule) {
                        itemLoot.Add(indexedRule.ToItemDropRule());
                    }
                }
                goto case ItemID.FrozenCrate;

            case ItemID.FrozenCrate:
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrozenTechnology>(), 2));
                goto BiomeCrate;

            // Other Crates
            case ItemID.DungeonFishingCrate:
            case ItemID.DungeonFishingCrateHard:
            case ItemID.HallowedFishingCrate:
            case ItemID.HallowedFishingCrateHard:
            case ItemID.JungleFishingCrate:
            case ItemID.LavaCrate:
            case ItemID.LavaCrateHard:
            case ItemID.OasisCrate:
            case ItemID.OasisCrateHard:
            case ItemID.OceanCrate:
            case ItemID.OceanCrateHard:
                goto BiomeCrate;

            case ItemID.CorruptFishingCrate:
            case ItemID.CorruptFishingCrateHard:
                goto BiomeCrate;

            case ItemID.CrimsonFishingCrate:
            case ItemID.CrimsonFishingCrateHard:
                goto BiomeCrate;

            case ItemID.FloatingIslandFishingCrate:
            case ItemID.FloatingIslandFishingCrateHard:
                goto BiomeCrate;

            case ItemID.LockBox:
                itemLoot.Add(ItemDropRule.OneFromOptions(1, ChestLootDataset.AequusDungeonChestLoot.ToArray()));
                break;

            BiomeCrate:
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                break;
        }
    }
}