using Aequus.Common.Items;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static bool Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
        return AequusRecipes.ItemIdHasPrefixRecipe.Contains(self.type) || orig(self);
    }
}
