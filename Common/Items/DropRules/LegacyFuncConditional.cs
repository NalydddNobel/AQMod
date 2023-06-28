using System;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.Items.DropRules {
    public class LegacyFuncConditional : FuncConditional {
        public LegacyFuncConditional(Func<bool> wasDefeated, string internalKey, string textKey = "Mods.Aequus.DropCondition.OnFirstKill") : base(wasDefeated, internalKey, textKey) {
        }

        public override bool CanDrop(DropAttemptInfo info) {
            return !base.CanDrop(info);
        }
    }
}