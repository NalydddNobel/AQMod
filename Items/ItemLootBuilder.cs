using Aequus.Common.ItemDrops;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public static class ItemLootBuilder
    {
        public struct Drops
        {
            public readonly int Item;
            public readonly ItemLoot Loot;
            public LeadingConditionRule LeadingConditionRule;

            public Drops(int item, ItemLoot loot)
            {
                Item = item;
                Loot = loot;
                LeadingConditionRule = null;
            }

            public Drops Add(IItemDropRule rule)
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

            public Drops Add(int itemID, int chance = 1, (int min, int max) stack = default((int, int)))
            {
                stack.Max(1);
                if (stack.min != stack.max)
                {
                    return Add(ItemDropRule.NotScalingWithLuck(itemID, chance, stack.min, stack.max));
                }
                return Add(ItemDropRule.Common(itemID, chance, stack.min, stack.max));
            }
            public Drops Add<T>(int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem
            {
                return Add(ModContent.ItemType<T>(), chance, stack);
            }
            public Drops Add(int itemID, int chance = 1, int stack = 1)
            {
                return Add(itemID, chance, (stack, stack));
            }
            public Drops Add<T>(int chance = 1, int stack = 1) where T : ModItem
            {
                return Add(ModContent.ItemType<T>(), chance, (stack, stack));
            }

            public Drops AddOptions(int chance, params int[] options)
            {
                return Add(ItemDropRule.OneFromOptions(chance, options));
            }

            public Drops Add(IItemDropRuleCondition condition, int itemID, (int chance, int over) dropChance = default((int, int)), (int min, int max) stack = default((int, int)))
            {
                stack.Max(1);
                dropChance.Max(1);
                return Add(ItemDropRule.ByCondition(condition, itemID, dropChance.chance, stack.min, stack.max, dropChance.over));
            }
            public Drops Add<T>(IItemDropRuleCondition condition, (int chance, int over) dropChance = default((int, int)), (int min, int max) stack = default((int, int))) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), dropChance, stack);
            }
            public Drops Add(IItemDropRuleCondition condition, int itemID, (int chance, int over) dropChance = default((int, int)), int stack = 1)
            {
                return Add(condition, itemID, dropChance, (stack, stack));
            }
            public Drops Add<T>(IItemDropRuleCondition condition, (int chance, int over) dropChance = default((int, int)), int stack = 1) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), dropChance, (stack, stack));
            }
            public Drops Add(IItemDropRuleCondition condition, int itemID, int chance = 1, (int min, int max) stack = default((int, int)))
            {
                return Add(condition, itemID, (chance, 1), stack);
            }
            public Drops Add<T>(IItemDropRuleCondition condition, int chance = 1, (int min, int max) stack = default((int, int))) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), (chance, 1), stack);
            }
            public Drops Add(IItemDropRuleCondition condition, int itemID, int chance = 1, int stack = 1)
            {
                return Add(condition, itemID, (chance, 1), (stack, stack));
            }
            public Drops Add<T>(IItemDropRuleCondition condition, int chance = 1, int stack = 1) where T : ModItem
            {
                return Add(condition, ModContent.ItemType<T>(), (chance, 1), (stack, stack));
            }

            public Drops Coins(int npcID)
            {
                return Add(ItemDropRule.CoinsBasedOnNPCValue(npcID));
            }
            public Drops Coins<T>() where T : ModNPC
            {
                return Coins(ModContent.NPCType<T>());
            }

            public Drops SetCondition(IItemDropRuleCondition rule)
            {
                LeadingConditionRule = new LeadingConditionRule(rule);
                return this;
            }
            public Drops RegisterCondition()
            {
                var condition = LeadingConditionRule;
                LeadingConditionRule = null;
                return Add(condition);
            }
        }

        public static Conditions.NotExpert NotExpertCondition => new Conditions.NotExpert();

        public static Drops AddLoot(this Item npc, ItemLoot loot)
        {
            return new Drops(npc.type, loot);
        }
        public static Drops CreateLoot(this ModItem modItem, ItemLoot loot)
        {
            return modItem.Item.AddLoot(loot);
        }
    }
}
