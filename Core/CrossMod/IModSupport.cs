namespace Aequus.Core.CrossMod;

internal interface IModSupport<TMod> where TMod : ModSupport<TMod> {
    public static Mod Instance { get; private set; }
    public static string ModName => typeof(TMod).Name;

    public bool IsLoadingEnabled() {
        return ModLoader.HasMod(typeof(TMod).Name);
    }
}
