using Aequus.Common.GlobalNPCs;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDropRules {
    public class GuaranteedFlawlesslyRule : IItemDropRule, IItemDropRuleCondition, IProvideItemConditionDescription
    {
        private readonly int Item;
        private readonly int DropChance;

        public List<IItemDropRuleChainAttempt> ChainedRules { get; private set; }

        public GuaranteedFlawlesslyRule(int item, int dropChance)
        {
            Item = item;
            DropChance = dropChance;
            ChainedRules = new List<IItemDropRuleChainAttempt>();
        }

        public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
        {
            float num = 1f / DropChance;
            float dropRate = num * ratesInfo.parentDroprateChance;
            ratesInfo.conditions = new List<IItemDropRuleCondition>() { this, };
            drops.Add(new DropRateInfo(Item, 1, 1, dropRate, ratesInfo.conditions));
            Chains.ReportDroprates(ChainedRules, num, drops, ratesInfo);
        }

        public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
        {
            ItemDropAttemptResult result;
            bool anyoneNoHit = false;
            var flags = info.npc.GetGlobalNPC<FlawlessGlobalNPC>().damagedPlayers;
            for (int i = 0; i < flags.Length; i++)
            {
                if (info.npc.playerInteraction[i] && !flags[i])
                {
                    anyoneNoHit = true;
                    break;
                }
            }
            if (anyoneNoHit || info.player.RollLuck(10) <= 1)
            {
                CommonCode.DropItem(info, Item, 1);
                result = default(ItemDropAttemptResult);
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            result = default(ItemDropAttemptResult);
            result.State = ItemDropAttemptResultState.FailedRandomRoll;
            return result;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return true;
        }

        bool IItemDropRuleCondition.CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return Language.GetTextValue("Mods.Aequus.DropCondition.Trophy");
        }
    }
}