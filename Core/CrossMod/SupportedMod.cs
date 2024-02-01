using System;
using System.Collections.Generic;

namespace Aequus.Core.CrossMod;

internal interface ISupportedMod<TMod> where TMod : SupportedMod<TMod> {
    public static Mod Instance { get; private set; }
    public static string ModName => typeof(TMod).Name;

    public bool IsLoadingEnabled() {
        return ModLoader.HasMod(typeof(TMod).Name);
    }
}

internal class SupportedMod<TMod> : ModSystem, ISupportedMod<TMod>, ILocalizedModType where TMod : SupportedMod<TMod> {
    /// <summary>The Supported Mod's instance. This property is <see langword="null" /> when the mod is not enabled.</summary>
    public static Mod Instance { get; private set; }
    public static string ModName => typeof(TMod).Name;
    public static bool IsEnabled => ModLoader.HasMod(ModName);

    public string LocalizationCategory => $"CrossMod.{ModName}";

    public static bool IsLoadingEnabled() {
        return IsEnabled;
    }

    public static object Call(params object[] args) {
        try {
            var value = Instance?.Call(args);
            if (value is Exception ex) {
                Aequus.Instance.Logger.Error(ex);
            }
            return value;
        }
        catch (Exception ex) {
            Aequus.Instance.Logger.Error(ex);
        }

        return null;
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

internal static class SupportedModExtensions {
    public static bool LoadingAllowed<TMod>(this ISupportedMod<TMod> modSupport) where TMod : SupportedMod<TMod> {
        return modSupport.IsLoadingEnabled();
    }

    public static bool TryFind<T, TMod>(this ISupportedMod<TMod> modSupport, string name, out T value) where T : IModType where TMod : SupportedMod<TMod> {
        var instance = SupportedMod<TMod>.Instance;
        if (instance == null) {
            value = default(T);
            return false;
        }
        return instance.TryFind(name, out value);
    }

    public static bool AddItemToSet<TMod>(this ISupportedMod<TMod> modSupport, string name, HashSet<int> hashSet) where TMod : SupportedMod<TMod> {
        if (!modSupport.TryFind<ModItem, TMod>(name, out var modItem)) {
            return false;
        }

        hashSet.Add(modItem.Type);
        return true;
    }

    public static int GetItem<TMod>(this ISupportedMod<TMod> modSupport, string name, int defaultItem = 0) where TMod : SupportedMod<TMod> {
        return modSupport.TryFind<ModItem, TMod>(name, out var value) ? value.Type : defaultItem;
    }
    public static int GetNPC<TMod>(this ISupportedMod<TMod> modSupport, string name, int defaultNPC = 0) where TMod : SupportedMod<TMod> {
        return modSupport.TryFind<ModNPC, TMod>(name, out var value) ? value.Type : defaultNPC;
    }
}