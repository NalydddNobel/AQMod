using System;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class OnFirstKillRule : IItemDropRule, IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public int ItemType;
        public Func<bool> wasDefeated;
        public readonly string Key;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public OnFirstKillRule(int item, Func<bool> wasDefeated, string defeatKey)
        {
            ItemType = item;
            this.wasDefeated = wasDefeated;
            Key = defeatKey;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            float num = 1f;
            float dropRate = num * ratesInfo.parentDroprateChance;
            ratesInfo.conditions = new List<IItemDropRuleCondition>() { this, };
            drops.Add(new DropRateInfo(ItemType, 1, 1, dropRate, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            var result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.DoesntFillConditions;

            if (wasDefeated())
            {
                CommonCode.DropItemFromNPC(info.npc, ItemType, 1);
                result.State = ItemDropAttemptResultState.Success;
            }
            return result;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.Aequus.DropCondition.OnFirstKill");
        }
    }
}