using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    public class ModSupportTemplate<T> : ModSystem where T : ModSupportTemplate<T>
    {
        public static Mod Instance { get; private set; }
        public static string ModName => typeof(T).Name;

        public override bool IsLoadingEnabled(Mod mod)
        {
            mod.Logger.Debug($"{ModName} is {(ModLoader.HasMod(ModName) ? "Enabled" : "Disabled")}");
            return ModLoader.HasMod(ModName);
        }

        public sealed override void Load()
        {
            Instance = null;
            if (ModLoader.TryGetMod(ModName, out var mod))
            {
                Instance = mod;
                SafeLoad(Instance);
            }
        }

        public virtual void SafeLoad(Mod mod)
        {

        }

        public sealed override void Unload()
        {
            SafeUnload();
            Instance = null;
        }

        public virtual void SafeUnload()
        {

        }
    }
}
