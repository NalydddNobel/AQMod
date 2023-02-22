using Aequus.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops
{
    public class NameTagDropRule : IItemDropRule
    {
        public ItemDrop itemDrop;
        public int chanceDenominator;
        public int chanceNumerator;
        public IItemDropRuleCondition condition;
        public string itemNameTag;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public NameTagDropRule(ItemDrop option, string text, IItemDropRuleCondition condition = null) : this(1, 1, option, text, condition)
        {
        }

        public NameTagDropRule(int chanceDenominator, int chanceNumerator, ItemDrop option, string text, IItemDropRuleCondition condition = null)
        {
            this.chanceDenominator = chanceDenominator;
            this.chanceNumerator = chanceNumerator;
            itemDrop = option;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
            this.condition = condition;
            itemNameTag = text;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return condition == null || condition.CanDrop(info);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(chanceDenominator) < chanceNumerator)
            {
                int i = Item.NewItem(info.npc.GetSource_Loot("Aequus: Name Easter Egg"), info.npc.getRect(), itemDrop.item, itemDrop.RollStack(info.rng));
                if (i >= 0 && i < Main.maxItems)
                {
                    if (Main.item[i].TryGetGlobalItem<AequusItem>(out var itemNameTag))
                    {
                        itemNameTag.NameTag = this.itemNameTag;
                    }
                }
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            var ratesInfo2 = ratesInfo.With(1f);
            if (condition != null)
            {
                ratesInfo2.AddCondition(condition);
            }
            float num = chanceNumerator / (float)chanceDenominator;
            drops.Add(new DropRateInfo(itemDrop.item, itemDrop.minStack, itemDrop.maxStack, 1f, ratesInfo2.conditions));

            Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo2);
        }
    }
}