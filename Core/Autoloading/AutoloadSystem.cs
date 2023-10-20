using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Aequus.Core.Autoloading;

internal class AutoloadSystem : ModSystem {
    public override void Load() {
        On_ItemDropDatabase.Populate += On_ItemDropDatabase_Populate;
        On_BestiaryDatabase.Merge += On_BestiaryDatabase_Merge;
    }

    private static void On_ItemDropDatabase_Populate(On_ItemDropDatabase.orig_Populate orig, ItemDropDatabase database) {
        orig(database);
        var aequus = Aequus.Instance;
        foreach (var t in aequus.GetContent<IPostPopulateItemDropDatabase>()) {
            t.PostPopulateItemDropDatabase(aequus, database);
        }
    }

    private static void On_BestiaryDatabase_Merge(On_BestiaryDatabase.orig_Merge orig, BestiaryDatabase bestiaryDatabase, ItemDropDatabase dropsDatabase) {
        var aequus = Aequus.Instance;
        foreach (var t in aequus.GetContent<IPreExtractBestiaryItemDrops>()) {
            t.PreExtractBestiaryItemDrops(aequus, bestiaryDatabase, dropsDatabase);
        }
        orig(bestiaryDatabase, dropsDatabase);
    }

    public override void PostSetupContent() {
        foreach (var t in Aequus.Instance.GetContent<IPostSetupContent>()) {
            t.PostSetupContent(Aequus.Instance);
        }
    }

    public override void AddRecipeGroups() {
        foreach (var t in Aequus.Instance.GetContent<IAddRecipeGroups>()) {
            t.AddRecipeGroups(Aequus.Instance);
        }
    }

    public override void AddRecipes() {
        foreach (var t in Aequus.Instance.GetContent<IAddRecipes>()) {
            t.AddRecipes(Aequus.Instance);
        }
    }

    public override void PostAddRecipes() {
        foreach (var t in Aequus.Instance.GetContent<IPostAddRecipes>()) {
            t.PostAddRecipes(Aequus.Instance);
        }
    }
}