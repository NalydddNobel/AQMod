using Aequus.Common.ItemDrops;
using Aequus.Items.Accessories.Summon;
using Aequus.Items.Consumables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs
{
    public static class DropRulesBuilder
    {
        public struct Drops
        {
            public readonly int NPC;
            public readonly NPCLoot Loot;
            public LeadingConditionRule LeadingConditionRule;

            public Drops(int npc, NPCLoot loot)
            {
                NPC = npc;
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

            public Drops AddBossBag(int itemID)
            {
                return Add(ItemDropRule.BossBag(itemID));
            }
            public Drops AddBossBag<T>() where T : TreasureBagBase
            {
                return AddBossBag(ModContent.ItemType<T>());
            }

            public Drops AddRelic(int itemID)
            {
                return Add(ItemDropRule.MasterModeCommonDrop(itemID));
            }
            public Drops AddRelic<T>() where T : ModItem
            {
                return AddRelic(ModContent.ItemType<T>());
            }

            public Drops AddMasterPet(int itemID)
            {
                return Add(ItemDropRule.MasterModeDropOnAllPlayers(itemID, 4));
            }
            public Drops AddMasterPet<T>() where T : ModItem
            {
                return AddMasterPet(ModContent.ItemType<T>());
            }

            public Drops AddBossLoot(int trophy, int relic, int bossBag = ItemID.None, int masterPet = ItemID.None)
            {
                Add(new GuaranteedFlawlesslyRule(trophy, 10));
                if (bossBag > 0)
                {
                    AddBossBag(bossBag);
                }
                AddRelic(relic);
                if (masterPet > 0)
                {
                    AddMasterPet(masterPet);
                }
                return this;
            }
            public Drops AddBossLoot<TTrophy, TRelic, TBossBag, TMasterPet>() where TTrophy : ModItem where TRelic : ModItem where TBossBag : ModItem where TMasterPet : ModItem
            {
                return AddBossLoot(ModContent.ItemType<TTrophy>(), ModContent.ItemType<TRelic>(), ModContent.ItemType<TBossBag>(), ModContent.ItemType<TMasterPet>());
            }
            public Drops AddBossLoot<TTrophy, TRelic, TBossBag>() where TTrophy : ModItem where TRelic : ModItem where TBossBag : TreasureBagBase
            {
                return AddBossLoot(ModContent.ItemType<TTrophy>(), ModContent.ItemType<TRelic>(), ModContent.ItemType<TBossBag>());
            }
            public Drops AddBossLoot<TTrophy, TRelic>() where TTrophy : ModItem where TRelic : ModItem
            {
                return AddBossLoot(ModContent.ItemType<TTrophy>(), ModContent.ItemType<TRelic>());
            }

            public Drops AddFlawless(int itemID)
            {
                return Add(ItemDropRule.ByCondition(new FlawlessCondition(), itemID));
            }
            public Drops AddFlawless<T>() where T : ModItem
            {
                return AddFlawless(ModContent.ItemType<T>());
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

        public static Drops AddLoot(this NPC npc, NPCLoot loot)
        {
            return new Drops(npc.type, loot);
        }
        public static Drops CreateLoot(this ModNPC modNPC, NPCLoot loot)
        {
            return AddLoot(modNPC.NPC, loot);
        }

        public class VanillaDrops : GlobalNPC
        {
            public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
            {
                if (npc.type == NPCID.Clown)
                {
                }
            }
        }
    }
}
