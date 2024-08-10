using Aequus.Common.Utilities;

namespace Aequus;
public partial class AequusItem {
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {
        if (LootBuilder.registerToItem.TryGetValue(item.type, out var list) && list != null) {
            foreach (var rule in list) {
                itemLoot.Add(rule);
            }
        }

        /*
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
                itemLoot.Add(ItemDropRule.OneFromOptions(1, HardmodeChestBoost.HardmodeChestLoot.ToArray()));
                goto case ItemID.GoldenCrate;
                break;

            case ItemID.GoldenCrate:
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SwordCursor>(), 4));
                break;

            // Iron and Mythril Crate Loot
            case ItemID.IronCrate:
            case ItemID.IronCrateHard:
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GlowCore>(), 6));
                itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MiningPetSpawner>(), 6));
                itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(4, ModContent.ItemType<Bellows>()));
                break;

            // Jungle and Bramble Crate loot
            case ItemID.JungleFishingCrateHard:
                itemLoot.Add(ItemDropRule.OneFromOptions(1, HardmodeChestBoost.HardmodeJungleChestLoot.ToArray()));
                goto case ItemID.JungleFishingCrate;
                break;

            // Frozen and Boreal Crate loot
            case ItemID.FrozenCrateHard:
                itemLoot.Add(ItemDropRule.OneFromOptions(1, HardmodeChestBoost.HardmodeSnowChestLoot.ToArray()));
                goto case ItemID.FrozenCrate;
                break;
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
        */
    }
}