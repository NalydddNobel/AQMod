using Aequus.Common.Preferences;
using Aequus.Common.Utilities;
using Aequus.Content.CursorDyes.Items;
using Aequus.Content.World;
using Aequus.Items.Accessories.Combat.Necro;
using Aequus.Items.Accessories.Misc;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Misc.Bait;
using Aequus.Items.Pets.Miner;
using Aequus.Items.Tools;
using Aequus.Items.Weapons.Melee.Misc.Valari;
using Aequus.Items.Weapons.Melee.Swords.IronLotus;
using Aequus.Items.Weapons.Necromancy.Candles;
using Aequus.Items.Weapons.Necromancy.Sceptres.Revenant;
using Aequus.Items.Weapons.Ranged.Misc.Slingshot;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items {
    public partial class AequusItem : GlobalItem, IPostSetupContent, IAddRecipes {
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
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, HardmodeChestBoost.HardmodeChestLoot.ToArray()));
                    goto case ItemID.GoldenCrate;

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

                // Frozen and Boreal Crate loot
                case ItemID.FrozenCrateHard:
                    itemLoot.Add(ItemDropRule.OneFromOptions(1, HardmodeChestBoost.HardmodeSnowChestLoot.ToArray()));
                    goto case ItemID.FrozenCrate;
                case ItemID.FrozenCrate:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrozenTechnology>(), 4));
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