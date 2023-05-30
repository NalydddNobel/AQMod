using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops {
    public class FuncConditional : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public Func<bool> condition;
        public string textKey;
        public readonly string Key;

        public FuncConditional(Func<bool> condition, string internalKeyIncasePeopleWantToKnowWhatThisIsChecking, string textKey = "Mods.Aequus.DropCondition.OnFirstKill")
        {
            this.condition = condition;
            Key = internalKeyIncasePeopleWantToKnowWhatThisIsChecking;
            this.textKey = textKey;
        }

        public virtual bool CanDrop(DropAttemptInfo info)
        {
            return condition();
        }

        public virtual bool CanShowItemDropInUI()
        {
            return true;
        }

        public virtual string GetConditionDescription()
        {
            return textKey == null ? null : Language.GetTextValue(textKey);
        }
    }
}