using Aequus.Common.Initialization;
using System;
using System.Linq;
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
            t.PostPopulateItemDropDatabase(database);
        }
    }

    private static void On_BestiaryDatabase_Merge(On_BestiaryDatabase.orig_Merge orig, BestiaryDatabase bestiaryDatabase, ItemDropDatabase dropsDatabase) {
        var aequus = Aequus.Instance;
        foreach (var t in aequus.GetContent<IPreExtractBestiaryItemDrops>()) {
            t.PreExtractBestiaryItemDrops(bestiaryDatabase, dropsDatabase);
        }
        orig(bestiaryDatabase, dropsDatabase);
    }

    public override void OnModLoad() {
        // Must use "ToArray" to create a fixed list of content, as the list of content may be edited in the methods called within the foreach, thus creating an error.
        foreach (ILoadable content in Mod.GetContent().ToArray()) {
            if (content is IOnModLoad contentOnModLoad) {
                contentOnModLoad.OnModLoad();
            }

            object[] attributes = content.GetType().GetCustomAttributes(inherit: false);
            foreach (Attribute attr in attributes.Cast<Attribute>()) {
                if (attr is IAttributeOnModLoad attributeOnModLoad) {
                    attributeOnModLoad.OnModLoad(Mod, content);
                }
            }
        }

        foreach (var t in Mod.GetContent<IOnModLoad>()) {
            t.OnModLoad();
        }
    }

    public override void PostSetupContent() {
        OnPostSetupContent?.Invoke();
        foreach (var t in Mod.GetContent<IPostSetupContent>()) {
            t.PostSetupContent();
        }
    }

    public override void AddRecipeGroups() {
        foreach (var t in Mod.GetContent<IAddRecipeGroups>()) {
            t.AddRecipeGroups();
        }
    }

    public override void AddRecipes() {
        foreach (var t in Mod.GetContent<IAddRecipes>()) {
            t.AddRecipes();
        }
    }

    public override void PostAddRecipes() {
        foreach (var t in Mod.GetContent<IPostAddRecipes>()) {
            t.PostAddRecipes();
        }
    }

    public override void PostSetupRecipes() {
        foreach (var t in Mod.GetContent<IPostSetupRecipes>()) {
            t.PostSetupRecipes();
        }
    }
}
