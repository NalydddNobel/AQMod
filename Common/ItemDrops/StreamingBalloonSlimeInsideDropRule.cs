using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    internal class StreamingBalloonSlimeInsideDropRule : IItemDropRule, IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public int Item;
        public int Chance;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public StreamingBalloonSlimeInsideDropRule(int item, int chance = 2)
        {
            Item = item;
            Chance = chance;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            float num = 1f / Chance;
            float dropRate = num * ratesInfo.parentDroprateChance;
            ratesInfo.conditions = new List<IItemDropRuleCondition>() { this, };
            drops.Add(new DropRateInfo(Item, 1, 1, dropRate, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            var result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.DoesntFillConditions;
            return result;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return false;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.Aequus.DropCondition.StreamingBalloonSlime");
        }
    }
}