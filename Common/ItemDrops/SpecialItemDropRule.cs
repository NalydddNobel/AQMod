using Aequus.NPCs;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Common.ItemDrops {
    public class SpecialItemDropRule : ItemDropRuleChanceBase {
        public int itemId;

        public SpecialItemDropRule(int itemID, int numerator, int demoninator = 1) : base(numerator, demoninator) {
            itemId = itemID;
        }

        public override void ReportDroprates(List<DropRateInfo> drops, ref DropRateInfoChainFeed ratesInfo, ref float personalDropRate) {
            base.ReportDroprates(drops, ref ratesInfo, ref personalDropRate);
        }

        public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
            if (info.npc == null || !info.npc.TryGetGlobalNPC<AequusNPC>(out var aequus) || aequus.specialItemDrop != itemId) {
                return new ItemDropAttemptResult() { State = ItemDropAttemptResultState.DoesntFillConditions, };
            }

            CommonCode.DropItem(info, itemId, 1);
            return new ItemDropAttemptResult() { State = ItemDropAttemptResultState.Success, };
        }
    }
}