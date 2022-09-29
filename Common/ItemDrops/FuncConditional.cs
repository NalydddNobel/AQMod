using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class FuncConditional : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public Func<bool> wasDefeated;
        public string textKey;
        public readonly string Key;

        public FuncConditional(Func<bool> wasDefeated, string internalKey, string textKey = "Mods.Aequus.DropCondition.OnFirstKill")
        {
            this.wasDefeated = wasDefeated;
            Key = internalKey;
            this.textKey = textKey;
        }

        public bool CanDrop(DropAttemptInfo info)
        {
            return !wasDefeated();
        }

        public bool CanShowItemDropInUI()
        {
            return true;
        }

        public string GetConditionDescription()
        {
            return textKey == null ? null : Language.GetTextValue(textKey);
        }
    }
}