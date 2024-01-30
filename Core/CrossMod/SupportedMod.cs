using System;
using System.Collections.Generic;

namespace Aequus.Core.CrossMod;

internal interface ISupportedMod<TMod> where TMod : SupportedMod<TMod> {
    public static Mod Instance { get; private set; }
    public static String ModName => typeof(TMod).Name;

    public Boolean IsLoadingEnabled() {
        return ModLoader.HasMod(typeof(TMod).Name);
    }
}

internal class SupportedMod<TMod> : ModSystem where TMod : SupportedMod<TMod> {
    public static Mod Instance { get; private set; }
    public static String ModName => typeof(TMod).Name;

    public static Boolean IsLoadingEnabled() {
        return ModLoader.HasMod(ModName);
    }

    public static Object Call(params Object[] args) {
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
    public static Boolean TryCall<T>(out T value, params Object[] args) {
        var callValue = Call(args);
        if (callValue is T output) {
            value = output;
            return true;
        }
        value = default(T);
        return false;
    }

    public static Boolean TryFind<T>(String name, out T value) where T : IModType {
        var instance = Instance;
        if (instance == null) {
            value = default(T);
            return false;
        }
        return instance.TryFind(name, out value);
    }

    public static Boolean AddItem(String name, HashSet<Int32> hashSet) {
        if (!TryFind<ModItem>(name, out var modItem)) {
            return false;
        }

        hashSet.Add(modItem.Type);
        return true;
    }

    public static Boolean TryGetItem(String name, out ModItem value) {
        return TryFind(name, out value);
    }
    public static Int32 TryGetItem(String name, Int32 defaultItem = 0) {
        return TryGetItem(name, out var value) ? value.Type : defaultItem;
    }

    public static Int32 GetNPC(String name, Int32 defaultNPC = 0) {
        return TryFind<ModNPC>(name, out var value) ? value.Type : defaultNPC;
    }

    public override Boolean IsLoadingEnabled(Mod mod) {
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
    public static Boolean LoadingAllowed<TMod>(this ISupportedMod<TMod> modSupport) where TMod : SupportedMod<TMod> {
        return modSupport.IsLoadingEnabled();
    }

    public static Boolean TryFind<T, TMod>(this ISupportedMod<TMod> modSupport, String name, out T value) where T : IModType where TMod : SupportedMod<TMod> {
        var instance = SupportedMod<TMod>.Instance;
        if (instance == null) {
            value = default(T);
            return false;
        }
        return instance.TryFind(name, out value);
    }

    public static Boolean AddItemToSet<TMod>(this ISupportedMod<TMod> modSupport, String name, HashSet<Int32> hashSet) where TMod : SupportedMod<TMod> {
        if (!modSupport.TryFind<ModItem, TMod>(name, out var modItem)) {
            return false;
        }

        hashSet.Add(modItem.Type);
        return true;
    }

    public static Int32 GetItem<TMod>(this ISupportedMod<TMod> modSupport, String name, Int32 defaultItem = 0) where TMod : SupportedMod<TMod> {
        return modSupport.TryFind<ModItem, TMod>(name, out var value) ? value.Type : defaultItem;
    }
    public static Int32 GetNPC<TMod>(this ISupportedMod<TMod> modSupport, String name, Int32 defaultNPC = 0) where TMod : SupportedMod<TMod> {
        return modSupport.TryFind<ModNPC, TMod>(name, out var value) ? value.Type : defaultNPC;
    }
}