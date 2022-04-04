﻿using Terraria;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops
{
    internal class WhenAllNPCsAreDeadCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        private readonly int type;

        public WhenAllNPCsAreDeadCondition(int type)
        {
            this.type = type;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return !NPC.AnyNPCs(type);
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return "Test test, you should see this!";
        }
    }
}