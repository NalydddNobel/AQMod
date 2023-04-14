using Aequus.NPCs;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Content.Elites {
    public class EliteDropCondition : IItemDropRuleCondition {
        public ElitePrefix[] wantedPrefixes;

        public EliteDropCondition(params ElitePrefix[] prefixes) {
            wantedPrefixes = prefixes;
        }

        public bool CanDrop(DropAttemptInfo info) {
            if (info.npc == null) {
                return true;
            }
            if (!info.npc.TryGetGlobalNPC<AequusNPC>(out var aequus)) {
                return false;
            }
            for (int i = 0; i < wantedPrefixes.Length; i++) {
                if (aequus.HasPrefix(wantedPrefixes[i])) {
                    return true;
                }
            }
            return false;
        }

        public bool CanShowItemDropInUI() {
            return false;
        }

        public string GetConditionDescription() {
            return null;
        }
    }
}