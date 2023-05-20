using Aequus.Common.ItemDrops;
using Aequus.Common.Preferences;
using Aequus.Content.CursorDyes.Items;
using Aequus.Content.Fishing.Bait;
using Aequus.Items.Accessories.BlackPlague;
using Aequus.Items.Accessories.Necro;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.Items.Weapons.Magic.Healer;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Melee.BattleAxe;
using Aequus.Items.Weapons.Melee.Heavy;
using Aequus.Items.Weapons.Melee.Thrown;
using Aequus.Items.Weapons.Necromancy.Candles;
using Aequus.Items.Weapons.Necromancy.Scepters;
using Aequus.Items.Weapons.Ranged;
using Aequus.Items.Weapons.Ranged.Misc;
using Aequus.Unused.Items;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items {
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
        private static IItemDropRuleCondition ConditionIsHardmode => new Conditions.IsHardmode();

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            switch (item.type)
            {
                case ItemID.EyeOfCthulhuBossBag:
                    {
                        if (!GameplayConfig.Instance.EyeOfCthulhuOres || !GameplayConfig.Instance.EyeOfCthulhuOreDropsDecrease)
                        {
                            break;
                        }
                        if (itemLoot.Find<ItemDropWithConditionRule>((i) => i.itemId == ItemID.DemoniteOre, out var itemDropRule))
                        {
                            itemDropRule.amountDroppedMinimum /= 2;
                            itemDropRule.amountDroppedMaximum /= 2;
                        }
                        if (itemLoot.Find((i) => i.itemId == ItemID.CrimtaneOre, out itemDropRule))
                        {
                            itemDropRule.amountDroppedMinimum /= 2;
                            itemDropRule.amountDroppedMaximum /= 2;
                        }
                    }
                    break;

                case ItemID.BossBagBetsy:
                    {
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

                case ItemID.TwinsBossBag:
                case ItemID.DestroyerBossBag:
                case ItemID.SkeletronPrimeBossBag:
                    itemLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "AllMechs", "Mods.Aequus.DropCondition.AllMechs"), ModContent.ItemType<TheReconstruction>()));
                    break;

                // Golden and Titanium Crate loot
                case ItemID.GoldenCrateHard:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1,
                            ItemID.DualHook,
                            ItemID.MagicDagger,
                            ItemID.TitanGlove,
                            ItemID.PhilosophersStone,
                            ItemID.CrossNecklace,
                            ItemID.StarCloak
                        ));
                    goto case ItemID.GoldenCrate;

                case ItemID.GoldenCrate:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SwordCursor>(), 4));
                    break;

                // Iron and Mythril Crate Loot
                case ItemID.IronCrate:
                case ItemID.IronCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GlowCore>(), 6));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MiningPetSpawner>(), 6));
                    itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(2, ModContent.ItemType<BoneHawkRing>(), ModContent.ItemType<Bellows>()));
                    break;

                // Jungle and Bramble Crate loot
                case ItemID.JungleFishingCrateHard:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1,
                            ModContent.ItemType<Nettlebane>(),
                            ModContent.ItemType<Hitscanner>(),
                            ModContent.ItemType<SavingGrace>()
                        ));
                    goto case ItemID.JungleFishingCrate;

                // Frozen and Boreal Crate loot
                case ItemID.FrozenCrateHard:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1,
                            ItemID.Frostbrand,
                            ItemID.IceBow,
                            ItemID.FlowerofFrost
                        ));
                    goto case ItemID.FrozenCrate;

                case ItemID.FrozenCrate:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrystalDagger>(), 3));
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
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CorruptionCandle>(), 3));
                    goto BiomeCrate;

                case ItemID.CrimsonFishingCrate:
                case ItemID.CrimsonFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrimsonCandle>(), 3));
                    goto BiomeCrate;

                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Slingshot>(), 3));
                    goto BiomeCrate;

                case ItemID.LockBox:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Valari>(), ModContent.ItemType<Revenant>(), ModContent.ItemType<DungeonCandle>(), ModContent.ItemType<PandorasBox>()));
                    break;

                BiomeCrate:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;
            }
        }
    }
}