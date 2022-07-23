using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class OnFirstKillCondition : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public Func<bool> wasDefeated;
        public readonly string Key;

        public OnFirstKillCondition(Func<bool> wasDefeated, string defeatKey)
        {
            this.wasDefeated = wasDefeated;
            Key = defeatKey;
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
            return Language.GetTextValue("Mods.Aequus.DropCondition.OnFirstKill");
        }
    }
}