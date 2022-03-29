using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.DropRules
{
    public sealed class TrophyDrop : IItemDropRule, IProvideItemConditionDescription
    {
        private readonly int Trophy;

        public TrophyDrop(int trophy)
        {
            Trophy = trophy;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            float num = 1f / 10f;
            float dropRate = num * ratesInfo.parentDroprateChance;
            drops.Add(new DropRateInfo(Trophy, 1, 1, dropRate, ratesInfo.conditions));
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;
            if (info.player.RollLuck(10) <= 1)
            {
                CommonCode.DropItemFromNPC(info.npc, Trophy, 1);
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.Aequus.DropCondition.Trophy");
        }
    }
}