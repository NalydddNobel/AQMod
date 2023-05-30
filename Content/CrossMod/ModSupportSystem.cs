namespace Aequus.Content.CrossMod {
    internal class ModSupportSystem
    {
        public static bool DoExpertDropsInClassicMode()
        {
            return CalamityMod.Instance != null;
        }
    }
}