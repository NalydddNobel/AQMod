using Aequus.Common.Items.DropRules;
using Aequus.Common.Preferences;
using Aequus.Common.Utilities;
using Aequus.Content.Elites;
using Aequus.Items.Equipment.Accessories.Combat;
using Aequus.Items.Equipment.Accessories.Combat.OnHitAbility.BoneRing;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.PossessedShard;
using Aequus.Items.Misc.PermanentUpgrades;
using Aequus.Items.Weapons.Melee.Misc.Mallet;
using Aequus.Items.Weapons.Melee.Misc.SickBeat;
using Aequus.Items.Weapons.Melee.Swords.CrystalDagger;
using Aequus.Items.Weapons.Melee.Swords.IronLotus;
using Aequus.Tiles.CraftingStations;
using System;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.NPCs;
public partial class AequusNPC {
    public static bool doLuckyDropsEffect;

    public bool noGravityDrops;
    public int specialItemDrop;

    private void Load_Drops() {
        On_ItemDropResolver.ResolveRule += ItemDropResolver_ResolveRule;
        On_NPC.NPCLoot_DropItems += NPC_NPCLoot_DropItems;
    }

    public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
        globalLoot.Add(ItemDropRule.ByCondition(new VictorsReward.DropCondition(), ModContent.ItemType<VictorsReward>()));
        globalLoot.Add(ItemDropRule.ByCondition(new EliteDropCondition(
            ModContent.GetInstance<ArgonElite>(),
            ModContent.GetInstance<KryptonElite>(),
            ModContent.GetInstance<NeonElite>(),
            ModContent.GetInstance<XenonElite>()
        ), ModContent.ItemType<GlowLichen>(), 1, 1, 3, 1));

        ItemDropWithConditionRule krakenRule = null;
        foreach (var r in globalLoot.Get()) {
            if (r is ItemDropWithConditionRule drop && drop.itemId == ItemID.Kraken) {
                krakenRule = drop;
            }
        }

        if (krakenRule != null) {
            globalLoot.Add(new Conditions.ZenithSeedIsNotUp(), krakenRule);
            globalLoot.Add(new Conditions.ZenithSeedIsUp(), ItemDropRule.Common(ModContent.ItemType<Mallet>(), krakenRule.chanceDenominator));
        }
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
        switch (npc.type) {
            case NPCID.UndeadViking:
            case NPCID.ArmoredViking: {
                    npcLoot.Add(new SpecialItemDropRule(ModContent.ItemType<CrystalDagger>(), 20));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrozenTechnology>(), 7));
                }
                break;

            case NPCID.ToxicSludge: {
                    npcLoot.Add(new SpecialItemDropRule(ModContent.ItemType<SickBeat>(), 10));
                }
                break;

            case NPCID.DarkCaster: {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoneRing>(), 15));
                }
                break;

            case NPCID.CursedHammer:
            case NPCID.CrimsonAxe:
                npcLoot.Add(new PossessedShardDropRule(1, min: 2, max: 3));
                break;

            case NPCID.PossessedArmor:
                npcLoot.Add(new PossessedShardDropRule(2));
                break;

            case NPCID.EyeofCthulhu: {
                    if (!GameplayConfig.Instance.EyeOfCthulhuOres) {
                        break;
                    }

                    npcLoot.RemoveWhere<ItemDropWithConditionRule>(r => r.itemId == ItemID.DemoniteOre);
                    npcLoot.RemoveWhere<ItemDropWithConditionRule>(r => r.itemId == ItemID.CrimtaneOre);
                }
                break;

            case NPCID.DD2Betsy: {
                    npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<IronLotus>()));
                }
                break;

            case NPCID.BloodNautilus:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 6, maximumDropped: 12));
                break;

            case NPCID.BloodEelHead:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 3, maximumDropped: 6));
                break;

            case NPCID.GoblinShark:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 3, maximumDropped: 6));
                break;

            case NPCID.EyeballFlyingFish:
            case NPCID.ZombieMerman:
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 1, maximumDropped: 3));
                break;

            case NPCID.MoonLordCore:
                if (GameplayConfig.Instance.EarlyGravityGlobe) {
                    npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.GravityGlobe);
                }
                if (GameplayConfig.Instance.EarlyPortalGun) {
                    npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.PortalGun);
                }
                break;

            case NPCID.CultistBoss:
                npcLoot.Add(ItemDropRule.ByCondition(LootBuilder.FlawlessCondition, ModContent.ItemType<MothmanMask>()));
                break;

            case NPCID.Plantera:
                npcLoot.Add(ItemDropRule.ByCondition(LootBuilder.NotExpertCondition, ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                break;

            case NPCID.WallofFlesh:
                npcLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => !AequusWorld.downedEventDemon, "DemonSiege", "Mods.Aequus.DropCondition.NotBeatenDemonSiege"), ModContent.ItemType<GoreNest>()));
                break;
        }
    }

    #region Hooks
    private static void NPC_NPCLoot_DropItems(On_NPC.orig_NPCLoot_DropItems orig, NPC self, Player closestPlayer) {
        var aequus = self.Aequus();
        var aequusPlayer = closestPlayer.Aequus();

        self.value *= 1f + aequusPlayer.increasedEnemyMoney;
        aequus.ProcFaultyCoin(self, aequus, closestPlayer, aequusPlayer);
        aequus.ProcFoolsGoldRing(self, aequus, closestPlayer, aequusPlayer);

        orig(self, closestPlayer);

        doLuckyDropsEffect = false;
    }

    private static ItemDropAttemptResult ItemDropResolver_ResolveRule(On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info) {

        var result = orig(self, rule, info);
        if (info.player == null || result.State != ItemDropAttemptResultState.FailedRandomRoll) {
            return result;
        }

        if (Helper.iterations != 0) {
            Helper.iterations++;
            return result;
        }

        float luckLeft = info.player.Aequus().dropRerolls;
        if (info.npc != null) {
            luckLeft += info.npc.Aequus().dropRerolls;
        }

        for (; luckLeft > 0f; luckLeft--) {
            if (luckLeft < 1f) {
                if (Main.rand.NextFloat(1f) > luckLeft) {
                    return result;
                }
            }

            doLuckyDropsEffect = true;
            Helper.iterations++;

            try {
                var result2 = orig(self, rule, info);
                if (result2.State != ItemDropAttemptResultState.FailedRandomRoll) {
                    Helper.iterations = 0;
                    return result2;
                }
            }
            catch (Exception ex) {
            }

            doLuckyDropsEffect = false;
        }
        Helper.iterations = 0;

        return result;
    }
    #endregion
}