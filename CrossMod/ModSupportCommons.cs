namespace Aequus.CrossMod {
    internal static class ModSupportCommons {
        public static bool DoExpertDropsInClassicMode() {
            return CalamityMod.Instance != null;
        }
    }
}