using Terraria.ModLoader;

namespace Aequus.Content.CrossMod
{
    internal class ModSupport<T> : ModSystem where T : ModSupport<T>
    {
        public static Mod Instance { get; private set; }
        public static string ModName => typeof(T).Name;

        public static bool IsLoadingEnabled()
        {
            return ModLoader.HasMod(ModName);
        }

        public override bool IsLoadingEnabled(Mod mod)
        {
            //mod.Logger.Debug($"{ModName} is {(ModLoader.HasMod(ModName) ? "Enabled" : "Disabled")}");
            return IsLoadingEnabled();
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
