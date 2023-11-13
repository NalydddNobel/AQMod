using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Common.CrossMod;

internal interface IModSupport<TMod> where TMod : ModSupport<TMod> {
    public static Mod Instance { get; private set; }
    public static string ModName => typeof(TMod).Name;

    public bool LoadingAllowed() {
        return ModLoader.HasMod(typeof(TMod).Name);
    }
}
internal class ModSupport<TMod> : ModSystem where TMod : ModSupport<TMod> {
    public static Mod Instance { get; private set; }
    public static string ModName => typeof(TMod).Name;

    public static bool IsLoadingEnabled() {
        return ModLoader.HasMod(ModName);
    }

    public static object Call(params object[] args) {
        return Instance?.Call(args);
    }
    public static bool TryCall<T>(out T value, params object[] args) {
        var callValue = Call(args);
        if (callValue is T output) {
            value = output;
            return true;
        }
        value = default(T);
        return false;
    }

    public static bool TryFind<T>(string name, out T value) where T : IModType {
        var instance = Instance;
        if (instance == null) {
            value = default(T);
            return false;
        }
        return instance.TryFind(name, out value);
    }

    public static bool AddItem(string name, HashSet<int> hashSet) {
        if (!TryFind<ModItem>(name, out var modItem)) {
            return false;
        }

        hashSet.Add(modItem.Type);
        return true;
    }

    public static bool TryGetItem(string name, out ModItem value) {
        return TryFind(name, out value);
    }
    public static int TryGetItem(string name, int defaultItem = 0) {
        return TryGetItem(name, out var value) ? value.Type : defaultItem;
    }

    public static int GetNPC(string name, int defaultNPC = 0) {
        return TryFind<ModNPC>(name, out var value) ? value.Type : defaultNPC;
    }

    public override bool IsLoadingEnabled(Mod mod) {
        //mod.Logger.Debug($"{ModName} is {(ModLoader.HasMod(ModName) ? "Enabled" : "Disabled")}");
        return IsLoadingEnabled();
    }

    public sealed override void Load() {
        Instance = null;
        if (ModLoader.TryGetMod(ModName, out var mod)) {
            Instance = mod;
            SafeLoad(Instance);
        }
    }

    public virtual void SafeLoad(Mod mod) {

    }

    public sealed override void Unload() {
        SafeUnload();
        Instance = null;
    }

    public virtual void SafeUnload() {

    }
}
internal static class ModSupportExtensions {
    public static bool LoadingAllowed<TMod>(this IModSupport<TMod> modSupport) where TMod : ModSupport<TMod> {
        return modSupport.LoadingAllowed();
    }

    public static bool TryFind<T, TMod>(this IModSupport<TMod> modSupport, string name, out T value) where T : IModType where TMod : ModSupport<TMod> {
        var instance = ModSupport<TMod>.Instance;
        if (instance == null) {
            value = default(T);
            return false;
        }
        return instance.TryFind(name, out value);
    }

    public static bool AddItemToSet<TMod>(this IModSupport<TMod> modSupport, string name, HashSet<int> hashSet) where TMod : ModSupport<TMod> {
        if (!modSupport.TryFind<ModItem, TMod>(name, out var modItem)) {
            return false;
        }

        hashSet.Add(modItem.Type);
        return true;
    }

    public static int GetItem<TMod>(this IModSupport<TMod> modSupport, string name, int defaultItem = 0) where TMod : ModSupport<TMod> {
        return modSupport.TryFind<ModItem, TMod>(name, out var value) ? value.Type : defaultItem;
    }
    public static int GetNPC<TMod>(this IModSupport<TMod> modSupport, string name, int defaultNPC = 0) where TMod : ModSupport<TMod> {
        return modSupport.TryFind<ModNPC, TMod>(name, out var value) ? value.Type : defaultNPC;
    }
}