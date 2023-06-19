using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Items {
    internal static class LegacyItemLootBuilder
    {
        public struct LegacyDropBuilder
        {
            public readonly int Item;
            public readonly ItemLoot Loot;
            public LeadingConditionRule LeadingConditionRule;

            public LegacyDropBuilder(int item, ItemLoot loot)
            {
                Item = item;
                Loot = loot;
                LeadingConditionRule = null;
            }

            public LegacyDropBuilder Add(IItemDropRule rule)
            {
                if (LeadingConditionRule != null)
                {
                    LeadingConditionRule.OnSuccess(rule);
                }
                else
                {
                    Loot.Add(rule);
                }
                return this;
            }

            public LegacyDropBuilder Add(int itemID, int chance = 1, (int min, int max) stack = default((int, int)))
            {
                stack.Max(1);
                if (stack.min != stack.max)
                {
                    return Add(ItemDropRule.NotScalingWithLuck(itemID, chance, stack.min, stack.max));
                }
                return Add(ItemDropRule.Common(itemID, chance, stack.min, stack.max));
            }
            public LegacyDropBuilder Add<T>(int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem
            {
                return Add(ModContent.ItemType<T>(), chance, stack);
            }
            public LegacyDropBuilder Add(int itemID, int chance = 1, int stack = 1)
            {
                return Add(itemID, chance, (stack, stack));
            }
            public LegacyDropBuilder Add<T>(int chance = 1, int stack = 1) where T : ModItem
            {
                return Add(ModContent.ItemType<T>(), chance, (stack, stack));
            }

            public LegacyDropBuilder AddOptions(int chance, params int[] options)
            {
                return Add(ItemDropRule.OneFromOptions(chance, options));
            }

            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, (int chance, int over) dropChance = default((int, int)), (int min, int max) stack = default((int, int)))
            {
                stack.Max(1);
                dropChance.Max(1);
                return Add(ItemDropRule.ByCondition(condition, itemID, dropChance.chance, stack.min, stack.max, dropChance.over));
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, (int chance, int over) dropChance = default((int, int)), (int min, int max) stack = default((int, int))) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), dropChance, stack);
            }
            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, (int chance, int over) dropChance = default((int, int)), int stack = 1)
            {
                return Add(condition, itemID, dropChance, (stack, stack));
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, (int chance, int over) dropChance = default((int, int)), int stack = 1) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), dropChance, (stack, stack));
            }
            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, int chance = 1, (int min, int max) stack = default((int, int)))
            {
                return Add(condition, itemID, (chance, 1), stack);
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), (chance, 1), stack);
            }
            public LegacyDropBuilder Add(IItemDropRuleCondition condition, int itemID, int chance = 1, int stack = 1)
            {
                return Add(condition, itemID, (chance, 1), (stack, stack));
            }
            public LegacyDropBuilder Add<T>(IItemDropRuleCondition condition, int chance = 1, int stack = 1) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), (chance, 1), (stack, stack));
            }

            public LegacyDropBuilder Coins(int npcID)
            {
                return Add(ItemDropRule.CoinsBasedOnNPCValue(npcID));
            }
            public LegacyDropBuilder Coins<T>() where T : ModNPC
            {
                return Coins(ModContent.NPCType<T>());
            }

            public LegacyDropBuilder SetCondition(IItemDropRuleCondition rule)
            {
                LeadingConditionRule = new LeadingConditionRule(rule);
                return this;
            }
            public LegacyDropBuilder RegisterCondition()
            {
                var condition = LeadingConditionRule;
                LeadingConditionRule = null;
                return Add(condition);
            }
        }

        public static Conditions.NotExpert NotExpertCondition => new Conditions.NotExpert();

        public static LegacyDropBuilder AddLoot(this Item npc, ItemLoot loot)
        {
            return new LegacyDropBuilder(npc.type, loot);
        }
        public static LegacyDropBuilder CreateLoot(this ModItem modItem, ItemLoot loot)
        {
            return modItem.Item.AddLoot(loot);
        }
    }
}
