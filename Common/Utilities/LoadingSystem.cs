using System;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;

namespace Aequus;

internal class LoadingSystem : ModSystem {
    public event Action? OnPostSetupContent;

    public override void Load() {
        On_ItemDropDatabase.Populate += On_ItemDropDatabase_Populate;
        On_BestiaryDatabase.Merge += On_BestiaryDatabase_Merge;
    }

    private static void On_ItemDropDatabase_Populate(On_ItemDropDatabase.orig_Populate orig, ItemDropDatabase database) {
        orig(database);
        var aequus = Aequus.Instance;
        foreach (var t in aequus.GetContent<IPostPopulateItemDropDatabase>()) {
            t.PostPopulateItemDropDatabase(Aequus.Instance, database);
        }
    }

    private static void On_BestiaryDatabase_Merge(On_BestiaryDatabase.orig_Merge orig, BestiaryDatabase bestiaryDatabase, ItemDropDatabase dropsDatabase) {
        var aequus = Aequus.Instance;
        foreach (var t in aequus.GetContent<IPreExtractBestiaryItemDrops>()) {
            t.PreExtractBestiaryItemDrops(Aequus.Instance, bestiaryDatabase, dropsDatabase);
        }
        orig(bestiaryDatabase, dropsDatabase);
    }

    public override void PostSetupContent() {
        OnPostSetupContent?.Invoke();
        foreach (var t in Mod.GetContent<IPostSetupContent>()) {
            t.PostSetupContent(Aequus.Instance);
        }
    }

    public override void AddRecipeGroups() {
        foreach (var t in Mod.GetContent<IAddRecipeGroups>()) {
            t.AddRecipeGroups(Aequus.Instance);
        }
    }

    public override void AddRecipes() {
        foreach (var t in Mod.GetContent<IAddRecipes>()) {
            t.AddRecipes(Aequus.Instance);
        }
    }

    public override void PostAddRecipes() {
        foreach (var t in Mod.GetContent<IPostAddRecipes>()) {
            t.PostAddRecipes(Aequus.Instance);
        }
    }

    public override void PostSetupRecipes() {
        foreach (var t in Mod.GetContent<IPostSetupRecipes>()) {
            t.PostSetupRecipes();
        }
    }
}

internal interface IOnModLoad : ILoadable {
    void OnModLoad(Aequus aequus);
}

internal interface IPostSetupContent : ILoadable {
    void PostSetupContent(Aequus aequus);
}

internal interface IAddRecipeGroups : ILoadable {
    void AddRecipeGroups(Aequus aequus);
}

internal interface IAddRecipes : ILoadable {
    void AddRecipes(Aequus aequus);
}

internal interface IPostAddRecipes : ILoadable {
    void PostAddRecipes(Aequus aequus);
}

internal interface IPostSetupRecipes : ILoadable {
    void PostSetupRecipes();
}

internal interface IPostPopulateItemDropDatabase : ILoadable {
    void PostPopulateItemDropDatabase(Aequus aequus, ItemDropDatabase database);
}

internal interface IPreExtractBestiaryItemDrops : ILoadable {
    void PreExtractBestiaryItemDrops(Aequus aequus, BestiaryDatabase bestiaryDatabase, ItemDropDatabase database);
}