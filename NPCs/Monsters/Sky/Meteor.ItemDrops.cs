using Aequus.Items.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Sky
{
    partial class Meteor : ModNPC
    {
        private abstract class BaseRule : IItemDropRule
        {
            public List<IItemDropRuleChainAttempt> ChainedRules { get; set; }

            public BaseRule()
            {
                ChainedRules = new List<IItemDropRuleChainAttempt>();
            }

            public virtual bool CanDrop(DropAttemptInfo info)
            {
                return true;
            }

            public abstract void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo);

            protected abstract void DropItem(DropAttemptInfo info);

            public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
            {
                ItemDropAttemptResult result;
                if ((int)info.npc.ai[0] != 2)
                {
                    DropItem(info);
                    result = default(ItemDropAttemptResult);
                    result.State = ItemDropAttemptResultState.Success;
                    return result;
                }

                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.FailedRandomRoll;
                return result;
            }
        }
        private sealed class StupidAltWorldConditionThing : IItemDropRuleCondition
        {
            private int tier;
            private bool isAlt;
            private int textType;

            public StupidAltWorldConditionThing(int tier, bool alt)
            {
                this.tier = tier;
                isAlt = alt;
                textType = tier * 2 + (alt ? 1 : 0);
            }

            public bool CanDrop(DropAttemptInfo info)
            {
                return CanShowItemDropInUI();
            }

            public bool CanShowItemDropInUI()
            {
                return Main.drunkWorld || UglyCodeForCheckingIfYouAreInAnAlternateMaterialWorldUsingMysteriousTierVariable(tier) == isAlt;
            }

            public string GetConditionDescription()
            {
                return Aequus.GetText("DropCondition.OreTier." + textType);
            }
        }
        private sealed class StupidOresAndBarsRule : BaseRule
        {
            public int ore;
            public int altOre;
            public int bar;
            public int altBar;
            public int tier;
            public int dropAmt;

            public StupidOresAndBarsRule(int ore, int altOre, int bar, int altBar, int tier, int dropAmt) : base()
            {
                this.ore = ore;
                this.altOre = altOre;
                this.bar = bar;
                this.altBar = altBar;
                this.tier = tier;
                this.dropAmt = dropAmt;
            }

            public override void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
            {
                var altRates = ratesInfo.With(1f);
                altRates.AddCondition(new StupidAltWorldConditionThing(tier, true));

                ratesInfo.AddCondition(new StupidAltWorldConditionThing(tier, false));

                drops.Add(new DropRateInfo(ore, 1, dropAmt - 1, 0.75f, ratesInfo.conditions));
                drops.Add(new DropRateInfo(bar, 1, 1, 0.25f, ratesInfo.conditions));

                drops.Add(new DropRateInfo(altOre, 1, dropAmt - 1, 0.75f, altRates.conditions));
                drops.Add(new DropRateInfo(altBar, 1, 1, 0.25f, altRates.conditions));

                Chains.ReportDroprates(ChainedRules, 1f, drops, ratesInfo);
            }

            protected override void DropItem(DropAttemptInfo info)
            {
                bool altOres = UglyCodeForCheckingIfYouAreInAnAlternateMaterialWorldUsingMysteriousTierVariable(tier);
                int stack = Main.rand.Next(dropAmt) + 1;
                if (stack == dropAmt)
                {
                    CommonCode.DropItemFromNPC(info.npc, altOres ? altBar : bar, 1);
                    return;
                }
                CommonCode.DropItemFromNPC(info.npc, altOres ? altOre : ore, stack);
            }
        }

        private static bool UglyCodeForCheckingIfYouAreInAnAlternateMaterialWorldUsingMysteriousTierVariable(int tier)
        {
            if (Main.drunkWorld)
            {
                return WorldGen.genRand.NextBool();
            }
            if (tier == 1)
            {
                return WorldGen.SavedOreTiers.Iron == TileID.Iron;
            }
            else if (tier == 2)
            {
                return WorldGen.SavedOreTiers.Silver == TileID.Silver;
            }
            else if (tier == 3)
            {
                return WorldGen.SavedOreTiers.Gold == TileID.Gold;
            }
            return WorldGen.SavedOreTiers.Copper == TileID.Copper;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(new CommonDrop(ModContent.ItemType<Pumpinator>(), 15));
            npcLoot.Add(new StupidOresAndBarsRule(ItemID.CopperOre, ItemID.TinOre, ItemID.CopperBar, ItemID.TinBar, 0, 3));
            npcLoot.Add(new StupidOresAndBarsRule(ItemID.IronOre, ItemID.LeadOre, ItemID.IronBar, ItemID.LeadBar, 1, 3));
            npcLoot.Add(new StupidOresAndBarsRule(ItemID.SilverOre, ItemID.TungstenOre, ItemID.SilverBar, ItemID.TungstenBar, 2, 4));
            npcLoot.Add(new StupidOresAndBarsRule(ItemID.GoldOre, ItemID.PlatinumOre, ItemID.GoldBar, ItemID.PlatinumBar, 3, 4));
        }
    }
}