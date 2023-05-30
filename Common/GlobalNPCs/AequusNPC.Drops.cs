using Aequus.Common.ItemDrops;
using Aequus.Common.Preferences;
using Aequus.Content.Biomes.GoreNest.Tiles;
using Aequus.Content.CursorDyes.Items;
using Aequus.Content.Elites;
using Aequus.Items.Accessories.Combat;
using Aequus.Items.Accessories.Combat.OnHit.Debuff;
using Aequus.Items.Accessories.Misc.Luck;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Consumables.Permanent;
using Aequus.Items.Materials;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Misc;
using Aequus.Items.Vanity;
using Aequus.Items.Weapons.Melee.Heavy;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs {
    public partial class AequusNPC : GlobalNPC, IPostSetupContent, IAddRecipes {
        public static bool doLuckyDropsEffect;

        public bool noGravityDrops;

        private void Load_Drops() {
            On_ItemDropResolver.ResolveRule += ItemDropResolver_ResolveRule;
            On_NPC.NPCLoot_DropItems += NPC_NPCLoot_DropItems;
        }

        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            globalLoot.Add(ItemDropRule.ByCondition(new VictorsReward.DropCondition(), ModContent.ItemType<VictorsReward>()));
            globalLoot.Add(ItemDropRule.ByCondition(new EliteDropCondition(
                ModContent.GetInstance<ArgonElite>(),
                ModContent.GetInstance<KryptonElite>(),
                ModContent.GetInstance<NeonElite>(),
                ModContent.GetInstance<XenonElite>()
            ), ModContent.ItemType<GlowLichen>(), 1, 1, 3, 1));
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            ModifyLoot_Mimics(npc, npcLoot);
            switch (npc.type)
            {
                case NPCID.DarkCaster: {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoneRing>(), 15));
                    }
                    break;

                case NPCID.CursedHammer:
                case NPCID.CrimsonAxe:
                    npcLoot.Add(new DropOneByOne(ModContent.ItemType<PossessedShard>(), new() { 
                        ChanceNumerator = 1, ChanceDenominator = 1,
                        MinimumItemDropsCount = 2, MaximumItemDropsCount = 3, 
                        MinimumStackPerChunkBase = 1, MaximumStackPerChunkBase = 1,
                        BonusMaxDropsPerChunkPerPlayer = 0, BonusMinDropsPerChunkPerPlayer = 0 
                    }));
                    break;

                case NPCID.PossessedArmor:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PossessedShard>(), 3));
                    break;

                case NPCID.IceQueen:
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<GiftingSpirit>(), 15));
                    }
                    break;

                case NPCID.SantaNK1:
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<XmasCursor>(), 15));
                    }
                    break;

                case NPCID.Everscream:
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<EyeGlint>(), 15));
                    }
                    break;

                case NPCID.Pumpking:
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<PumpkingCursor>(), 20));
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.FromCertainWaveAndAbove(15), ModContent.ItemType<PumpkingCloak>(), 20));
                    }
                    break;

                case NPCID.HeadlessHorseman:
                    {
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Headless>(), 20));
                    }
                    break;

                case NPCID.EyeofCthulhu: {
                        if (!GameplayConfig.Instance.EyeOfCthulhuOres || !GameplayConfig.Instance.EyeOfCthulhuOreDropsDecrease) {
                            break;
                        }
                        npcLoot.RemoveWhere<ItemDropWithConditionRule>((i) => i.itemId == ItemID.DemoniteOre);
                        npcLoot.RemoveWhere<ItemDropWithConditionRule>((i) => i.itemId == ItemID.CrimtaneOre);
                    }
                    break;

                case NPCID.DD2Betsy:
                    {
                        npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<IronLotus>()));
                    }
                    break;

                case NPCID.Bunny:
                case NPCID.ExplosiveBunny:
                case NPCID.BunnyXmas:
                case NPCID.BunnySlimed:
                    npcLoot.Add(new NameTagDropRule(new(ModContent.ItemType<RabbitsFoot>(), 1), "You're a Monster.", new NameTagCondition("toast")));
                    break;

                case NPCID.Unicorn:
                    npcLoot.Add(new NameTagDropRule(new(ModContent.ItemType<RabbitsFoot>(), 1), "Tattered Pegasus Wings", new NameTagCondition("pegasus")));
                    break;

                case NPCID.Crab:
                    npcLoot.Add(new NameTagDropRule(new(ItemID.GoldCoin, 1), "Me first dollar!", new NameTagCondition("mr krabs", "krabs", "mr. krabs")));
                    break;

                case NPCID.Demon:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonCursor>(), 100));
                    break;

                case NPCID.Pixie:
                    //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PixieCandle>(), 100));
                    break;

                case NPCID.BloodNautilus:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 6, maximumDropped: 12));
                    break;

                case NPCID.BloodEelHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 3, maximumDropped: 6));
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 10));
                    break;

                case NPCID.GoblinShark:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 3, maximumDropped: 6));
                    break;

                case NPCID.EyeballFlyingFish:
                case NPCID.ZombieMerman:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTearstone>(), minimumDropped: 1, maximumDropped: 3));
                    break;

                case NPCID.DevourerHead:
                case NPCID.GiantWormHead:
                case NPCID.BoneSerpentHead:
                case NPCID.TombCrawlerHead:
                case NPCID.DiggerHead:
                case NPCID.DuneSplicerHead:
                case NPCID.SeekerHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpicyEel>(), 10));
                    break;

                case NPCID.MoonLordCore:
                    if (GameplayConfig.Instance.EarlyGravityGlobe)
                        npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.GravityGlobe);
                    if (GameplayConfig.Instance.EarlyPortalGun)
                        npcLoot.RemoveWhere((itemDrop) => itemDrop is ItemDropWithConditionRule dropRule && dropRule.itemId == ItemID.PortalGun);
                    break;

                case NPCID.CultistBoss:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.FlawlessCondition, ModContent.ItemType<MothmanMask>()));
                    break;

                case NPCID.Plantera:
                    npcLoot.Add(ItemDropRule.ByCondition(DropRulesBuilder.NotExpertCondition, ModContent.ItemType<OrganicEnergy>(), 1, 3, 3));
                    break;

                case NPCID.WallofFlesh:
                    npcLoot.Add(ItemDropRule.ByCondition(new FuncConditional(() => !AequusWorld.downedEventDemon, "DemonSiege", "Mods.Aequus.DropCondition.NotBeatenDemonSiege"), ModContent.ItemType<GoreNest>()));
                    break;
            }
        }

        #region Hooks
        private static void NPC_NPCLoot_DropItems(Terraria.On_NPC.orig_NPCLoot_DropItems orig, NPC self, Player closestPlayer) {
            var aequus = self.Aequus();
            var aequusPlayer = closestPlayer.Aequus();
            
            self.value *= 1f + aequusPlayer.increasedEnemyMoney;
            aequus.ProcFaultyCoin(self, aequus, closestPlayer, aequusPlayer);
            aequus.ProcFoolsGoldRing(self, aequus, closestPlayer, aequusPlayer);

            orig(self, closestPlayer);

            doLuckyDropsEffect = false;
        }

        private static ItemDropAttemptResult ItemDropResolver_ResolveRule(Terraria.GameContent.ItemDropRules.On_ItemDropResolver.orig_ResolveRule orig, ItemDropResolver self, IItemDropRule rule, DropAttemptInfo info) {

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
                catch(Exception ex) {
                }
                
                doLuckyDropsEffect = false;
            }
            Helper.iterations = 0;

            return result;
        }
        #endregion
    }

    internal static partial class GlobalNPCExtensions
    {
        public static void LockDrops(int npcID, IItemDropRuleCondition conditon, Func<IItemDropRule, bool> check)
        {
            var rules = Main.ItemDropsDB.GetRulesForNPCID(npcID);
            var badRules = new List<IItemDropRule>();
            for (int i = 0; i < rules.Count; i++)
            {
                if (check(rules[i]))
                {
                    badRules.Add(rules[i]);
                }
            }
            foreach (var r in badRules)
            {
                Main.ItemDropsDB.RemoveFromNPC(npcID, r);
                Main.ItemDropsDB.RegisterToNPC(npcID, new LeadingConditionRule(conditon)).OnSuccess(r);
            }
        }
    }
}