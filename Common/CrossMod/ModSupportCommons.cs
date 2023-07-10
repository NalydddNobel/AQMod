using Aequus.CrossMod;

namespace Aequus.Common.CrossMod {
    internal static class ModSupportCommons {
        public static bool DoExpertDropsInClassicMode() {
            return CalamityMod.Instance != null;
        }
    }
}