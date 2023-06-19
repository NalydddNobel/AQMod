using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDropRules {
    public class FuncConditional : IItemDropRuleCondition, IProvideItemConditionDescription {
        public Func<bool> condition;
        public string textKey;
        public readonly string Key;

        public FuncConditional(Func<bool> condition, string name, string textKey = null) {
            this.condition = condition;
            Key = name;
            if (!string.IsNullOrEmpty(textKey)) {
                this.textKey = "Mods.Aequus.DropCondition." + textKey;
            }
        }

        public virtual bool CanDrop(DropAttemptInfo info) {
            return condition();
        }

        public virtual bool CanShowItemDropInUI() {
            return true;
        }

        public virtual string GetConditionDescription() {
            return textKey == null ? null : Language.GetTextValue(textKey);
        }
    }
}