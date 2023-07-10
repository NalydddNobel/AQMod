using Aequus.Common.CrossMod;
using Aequus.Common.Items.DropRules;
using Aequus.CrossMod;
using Aequus.Items.Consumables.TreasureBag;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Channels;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Common.Utilities {
    public static class LootBuilder {
        public const int DroprateMask = 7;
        public const int DroprateTrophy = 10;
        public const int DroprateMasterPet = 4;
        
        internal static readonly Dictionary<int, List<IItemDropRule>> registerToItem = new();

        private class Loader : ILoadable {
            public void Load(Mod mod) {
                registerToItem.Clear();
            }

            public void Unload() {
                registerToItem.Clear();
            }
        }

        public static IItemDropRule Add(this ILoot loot, IItemDropRuleCondition condition, IItemDropRule dropRule) {
            var leadingConditionRule = new LeadingConditionRule(condition);
            return loot.Add(leadingConditionRule).OnSuccess(dropRule);
        }

        public static IItemDropRule Add<TCondition>(this ILoot loot, IItemDropRule dropRule) where TCondition : IItemDropRuleCondition, new() {
            return loot.Add(new TCondition(), dropRule);
        }

        #region Item Registering Hack
        /// <summary>Adds rule to <paramref name="loot"/>, and to the Item specified by <paramref name="bossBagId"/>. <paramref name="loot"/>'s rule will be conditioned to only drop in Classic (Normal) Mode.</summary>
        public static void AddBossLoot(this ILoot loot, int bossBagId, IItemDropRule dropRuleForNPC, IItemDropRule dropRuleForBag) {
            var leadingConditionRule = new LeadingConditionRule(new Conditions.NotExpert());
            loot.Add(leadingConditionRule).OnSuccess(dropRuleForNPC);
            if (registerToItem.TryGetValue(bossBagId, out var list)) {
                list.Add(dropRuleForBag);
            }
            else {
                registerToItem[bossBagId] = new() { dropRuleForBag };
            }
        }

        /// <summary>Adds rule to <paramref name="loot"/>, and to the Item specified by <paramref name="bossBagId"/>. <paramref name="loot"/>'s rule will be conditioned to only drop in Classic (Normal) Mode.</summary>
        public static IItemDropRule AddBossLoot(this ILoot loot, int bossBagId, IItemDropRule dropRule) {
            loot.AddBossLoot(bossBagId, dropRuleForNPC: dropRule, dropRuleForBag: dropRule);
            return dropRule;
        }
        #endregion

        #region Misc Register Methods
        public static IItemDropRule AddExpertDrop<T>(this ILoot loot, int bossBag) where T : ModItem {
            var dropRule = ItemDropRule.Common(ModContent.ItemType<T>());
            if (ModSupportCommons.DoExpertDropsInClassicMode()) {
                return loot.Add<Conditions.NotExpert>(dropRule);
            }

            loot.AddBossLoot(bossBag, dropRule);
            return dropRule;
        }
        #endregion

        #region Get IItemDropRuleCondition Methods
        public static IItemDropRuleCondition GetCondition_OnFirstKill(Func<bool> wasDefeated) {
            return new OnFirstKillCondition(wasDefeated);
        }
        #endregion

        #region Get IItemDropRule Methods
        public static IItemDropRule GetDropRule_PerPlayerInstanced(int itemID, int chance = 1, int min = 1, int max = 1, IItemDropRuleCondition condition = null) {
            return new DropPerPlayerOnThePlayer(itemID, chance, min, max, condition);
        }
        public static IItemDropRule GetDropRule_PerPlayerInstanced<T>(int chance = 1, int min = 1, int max = 1, IItemDropRuleCondition condition = null) where T : ModItem {
            return GetDropRule_PerPlayerInstanced(ModContent.ItemType<T>(), chance, min, max, condition);
        }
        #endregion

        public struct LegacyDropBuilder {
            public readonly int NPC;
            public readonly NPCLoot Loot;
            public LeadingConditionRule LeadingConditionRule;

            public LegacyDropBuilder(int npc, NPCLoot loot) {
                NPC = npc;
                Loot = loot;
                LeadingConditionRule = null;
            }

            public LegacyDropBuilder Add(IItemDropRule rule) {
                if (LeadingConditionRule != null) {
                    LeadingConditionRule.OnSuccess(rule);
                }
                else {
                    Loot.Add(rule);
                }
                return this;
            }

            public LegacyDropBuilder Add(int itemID, int chance = 1, (int min, int max) stack = default((int, int))) {
                stack.Max(1);
                return Add(ItemDropRule.Common(itemID, chance, stack.min, stack.max));
            }
            public LegacyDropBuilder Add<T>(int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem {
                return Add(ModContent.ItemType<T>(), chance, stack);
            }
            public LegacyDropBuilder Add(int itemID, int chance = 1, int stack = 1) {
                return Add(itemID, chance, (stack, stack));
            }
            public LegacyDropBuilder Add<T>(int chance = 1, int stack = 1) where T : ModItem {
                return Add(ModContent.ItemType<T>(), chance, (stack, stack));
            }

            public LegacyDropBuilder AddOptions(int chance, params int[] options) {
                return Add(ItemDropRule.OneFromOptions(chance, options));
            }

            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, (int chance, int over) dropChance = default((int, int)), (int min, int max) stack = default((int, int))) {
                stack.Max(1);
                dropChance.Max(1);
                return Add(ItemDropRule.ByCondition(condition, itemID, dropChance.chance, stack.min, stack.max, dropChance.over));
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, (int chance, int over) dropChance = default((int, int)), (int min, int max) stack = default((int, int))) where T : ModItem {
                return Add(condition, ModContent.ItemType<T>(), dropChance, stack);
            }
            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, (int chance, int over) dropChance = default((int, int)), int stack = 1) {
                return Add(condition, itemID, dropChance, (stack, stack));
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, (int chance, int over) dropChance = default((int, int)), int stack = 1) where T : ModItem {
                return Add(condition, ModContent.ItemType<T>(), dropChance, (stack, stack));
            }
            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, int chance = 1, (int min, int max) stack = default((int, int))) {
                return Add(condition, itemID, (chance, 1), stack);
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem {
                return Add(condition, ModContent.ItemType<T>(), (chance, 1), stack);
            }
            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, int chance = 1, int stack = 1) {
                return Add(condition, itemID, (chance, 1), (stack, stack));
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, int chance = 1, int stack = 1) where T : ModItem {
                return Add(condition, ModContent.ItemType<T>(), (chance, 1), (stack, stack));
            }

            public LegacyDropBuilder AddPerPlayer<T>(int chance = 1, int stack = 1) where T : ModItem {
                return AddPerPlayer(null, ModContent.ItemType<T>(), chance, stack);
            }
            public LegacyDropBuilder AddPerPlayer(int itemID, int chance = 1, int stack = 1) {
                return AddPerPlayer(null, itemID, chance, (stack, stack));
            }
            public LegacyDropBuilder AddPerPlayer<T>(int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem {
                return AddPerPlayer(null, ModContent.ItemType<T>(), chance, stack);
            }
            public LegacyDropBuilder AddPerPlayer(int itemID, int chance = 1, (int min, int max) stack = default((int, int))) {
                return Add(null, itemID, chance, stack);
            }
            public LegacyDropBuilder AddPerPlayer<T>(IItemDropRuleCondition condition, int chance = 1, int stack = 1) where T : ModItem {
                return AddPerPlayer(condition, ModContent.ItemType<T>(), chance, stack);
            }
            public LegacyDropBuilder AddPerPlayer(IItemDropRuleCondition condition, int itemID, int chance = 1, int stack = 1) {
                return AddPerPlayer(condition, itemID, chance, (stack, stack));
            }
            public LegacyDropBuilder AddPerPlayer<T>(IItemDropRuleCondition condition, int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem {
                return AddPerPlayer(condition, ModContent.ItemType<T>(), chance, stack);
            }
            public LegacyDropBuilder AddPerPlayer(IItemDropRuleCondition condition, int itemID, int chance = 1, (int min, int max) stack = default((int, int))) {
                return Add(new DropPerPlayerOnThePlayer(itemID, chance, stack.min, stack.max, condition));
            }

            public LegacyDropBuilder AddBossBag(int itemID) {
                return Add(ItemDropRule.BossBag(itemID));
            }
            public LegacyDropBuilder AddBossBag<T>() where T : TreasureBagBase {
                return AddBossBag(ModContent.ItemType<T>());
            }

            public LegacyDropBuilder AddRelic(int itemID) {
                if (CalamityMod.Instance != null) {
                    return Add(ItemDropRule.ByCondition(RevengeanceOrMasterMode, itemID));
                }
                return Add(ItemDropRule.MasterModeCommonDrop(itemID));
            }
            public LegacyDropBuilder AddRelic<T>() where T : ModItem {
                return AddRelic(ModContent.ItemType<T>());
            }

            public LegacyDropBuilder AddMasterPet(int itemID) {
                if (CalamityMod.Instance != null) {
                    return Add(new DropPerPlayerOnThePlayer(itemID, 4, 1, 1, RevengeanceOrMasterMode));
                }
                return Add(ItemDropRule.MasterModeDropOnAllPlayers(itemID, 4));
            }
            public LegacyDropBuilder AddMasterPet<T>() where T : ModItem {
                return AddMasterPet(ModContent.ItemType<T>());
            }

            public LegacyDropBuilder AddBossLoot(int trophy, int relic, int bossBag = ItemID.None, int masterPet = ItemID.None) {
                Add(new GuaranteedFlawlesslyRule(trophy, 10));
                if (bossBag > 0) {
                    AddBossBag(bossBag);
                }
                AddRelic(relic);
                if (masterPet > 0) {
                    AddMasterPet(masterPet);
                }
                return this;
            }
            public LegacyDropBuilder AddBossLoot<TTrophy, TRelic, TBossBag, TMasterPet>() where TTrophy : ModItem where TRelic : ModItem where TBossBag : ModItem where TMasterPet : ModItem {
                return AddBossLoot(ModContent.ItemType<TTrophy>(), ModContent.ItemType<TRelic>(), ModContent.ItemType<TBossBag>(), ModContent.ItemType<TMasterPet>());
            }
            public LegacyDropBuilder AddBossLoot<TTrophy, TRelic, TBossBag>() where TTrophy : ModItem where TRelic : ModItem where TBossBag : TreasureBagBase {
                return AddBossLoot(ModContent.ItemType<TTrophy>(), ModContent.ItemType<TRelic>(), ModContent.ItemType<TBossBag>());
            }
            public LegacyDropBuilder AddBossLoot<TTrophy, TRelic>() where TTrophy : ModItem where TRelic : ModItem {
                return AddBossLoot(ModContent.ItemType<TTrophy>(), ModContent.ItemType<TRelic>());
            }

            public LegacyDropBuilder ExpertDropForCrossModReasons<TExpertDrop>() where TExpertDrop : ModItem {
                if (ModSupportCommons.DoExpertDropsInClassicMode()) {
                    Add<TExpertDrop>(new Conditions.NotExpert(), 1, (1, 1));
                }
                return this;
            }

            public LegacyDropBuilder AddFlawless(int itemID) {
                return Add(ItemDropRule.ByCondition(FlawlessCondition, itemID));
            }
            public LegacyDropBuilder AddFlawless<T>() where T : ModItem {
                return AddFlawless(ModContent.ItemType<T>());
            }

            public LegacyDropBuilder SetCondition(IItemDropRuleCondition rule) {
                LeadingConditionRule = new LeadingConditionRule(rule);
                return this;
            }
            public LegacyDropBuilder RegisterCondition() {
                var condition = LeadingConditionRule;
                LeadingConditionRule = null;
                return Add(condition);
            }
        }

        private static IItemDropRuleCondition RevengeanceOrMasterMode => new FuncConditional(() => CalamityMod.Revengeance || Main.masterMode, "Revengeance Drop", "This is a Master or Revengeance Mode drop rate");
        public static Conditions.NotExpert NotExpertCondition => new Conditions.NotExpert();
        public static FlawlessCondition FlawlessCondition => new FlawlessCondition();

        public static LegacyDropBuilder AddLoot(this NPC npc, NPCLoot loot) {
            return new LegacyDropBuilder(npc.type, loot);
        }
        public static LegacyDropBuilder CreateLoot(this ModNPC modNPC, NPCLoot loot) {
            return modNPC.NPC.AddLoot(loot);
        }
    }
}