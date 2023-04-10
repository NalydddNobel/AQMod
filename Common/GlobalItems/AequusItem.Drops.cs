using Aequus.Common.ItemDrops;
using Aequus.Common.Preferences;
using Aequus.Content.CursorDyes.Items;
using Aequus.Content.Fishing.Bait;
using Aequus.Items.Accessories.Debuff;
using Aequus.Items.Accessories.Misc;
using Aequus.Items.Accessories.Offense.Necro;
using Aequus.Items.Accessories.Utility;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Melee.Heavy;
using Aequus.Items.Weapons.Melee.Thrown;
using Aequus.Items.Weapons.Ranged.Misc;
using Aequus.Items.Weapons.Summon.Candles;
using Aequus.Items.Weapons.Summon.Scepters;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items {
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes
    {
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

                case ItemID.GoldenCrate:
                case ItemID.GoldenCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SwordCursor>(), 4));
                    break;

                case ItemID.IronCrate:
                case ItemID.IronCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GlowCore>(), 6));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MiningPetSpawner>(), 6));
                    itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(2, ModContent.ItemType<BoneHawkRing>(), ModContent.ItemType<BattleAxe>(), ModContent.ItemType<Bellows>()));
                    break;

                case ItemID.DungeonFishingCrate:
                case ItemID.DungeonFishingCrateHard:
                case ItemID.HallowedFishingCrate:
                case ItemID.HallowedFishingCrateHard:
                case ItemID.JungleFishingCrate:
                case ItemID.JungleFishingCrateHard:
                case ItemID.LavaCrate:
                case ItemID.LavaCrateHard:
                case ItemID.OasisCrate:
                case ItemID.OasisCrateHard:
                case ItemID.OceanCrate:
                case ItemID.OceanCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.CorruptFishingCrate:
                case ItemID.CorruptFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CorruptionCandle>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.CrimsonFishingCrate:
                case ItemID.CrimsonFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrimsonCandle>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Slingshot>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrystalDagger>(), 3));
                    itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CrateBait>()));
                    break;

                case ItemID.LockBox:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, ModContent.ItemType<Valari>(), ModContent.ItemType<Revenant>(), ModContent.ItemType<DungeonCandle>(), ModContent.ItemType<PandorasBox>()));
                    break;
            }
        }
    }
}