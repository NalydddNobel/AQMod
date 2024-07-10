using AequusRemake.Core.Structures.ID;
using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace AequusRemake.Systems.CrossMod;

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
    public static bool Enabled { get; private set; }

    public string LocalizationCategory => $"CrossMod";

    public static bool IsLoadingEnabled() {
        return ModLoader.HasMod(ModName);
    }

    public static object Call(params object[] args) {
        try {
            var value = Instance?.Call(args);
            if (value is Exception ex) {
                Log.Error(ex);
            }
            return value;
        }
        catch (Exception ex) {
            Log.Error(ex);
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

    public static IProvideId GetContentProvider<T>(string name, int defaultItem = 0) where T : IModType {
        return new ModId<T>(name, ModName, defaultItem);
    }

    public static string GetLocalizationKey(string suffix) {
        return $"Mods.AequusRemake.CrossMod.{ModName}.{suffix}";
    }
    public static LocalizedText GetLocalization(string suffix) {
        return Language.GetOrRegister(GetLocalizationKey(suffix));
    }
    public static string GetLocalizedValue(string suffix) {
        return Language.GetTextValue(GetLocalizationKey(suffix));
    }

    public override bool IsLoadingEnabled(Mod mod) {
        //mod.Logger.Debug($"{ModName} is {(ModLoader.HasMod(ModName) ? "Enabled" : "Disabled")}");
        return IsLoadingEnabled();
    }

    public sealed override void Load() {
        if (ModLoader.TryGetMod(ModName, out var mod)) {
            Instance = mod;
            Enabled = true;
            OnLoad(Instance);
        }
        else {
            Enabled = false;
            Instance = null;
        }
    }

    public virtual void OnLoad(Mod mod) {

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

    public static Recipe SortBeforeModItem(this Recipe recipe, string name, CrossModItem crossModItem) {
        AequusRemake.OnPostAddRecipes += () => {
            if (ModLoader.TryGetMod(crossModItem.CrossModName, out Mod mod) && mod.TryFind(name, out ModItem modItem)) {
                recipe.SortBeforeFirstRecipesOf(modItem.Type);
            }
        };

        return recipe;
    }

    public static Recipe SortAfterModItem(this Recipe recipe, string name, CrossModItem crossModItem) {
        AequusRemake.OnPostAddRecipes += () => {
            if (ModLoader.TryGetMod(crossModItem.CrossModName, out Mod mod) && mod.TryFind(name, out ModItem modItem)) {
                recipe.SortAfterFirstRecipesOf(modItem.Type);
            }
        };

        return recipe;
    }
}