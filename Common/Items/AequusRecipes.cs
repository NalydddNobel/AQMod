using System.Collections.Generic;

namespace Aequus.Common.Items;

public sealed partial class AequusRecipes : ModSystem {
    public static readonly HashSet<System.Int32> ItemIdHasPrefixRecipe = new();
    public static readonly Dictionary<System.Int32, Condition> ShimmerTransformLocks = new();

    public override void Load() {
        On_Item.CanHavePrefixes += Item_CanHavePrefixes;
    }

    public override void Unload() {
        ItemIdHasPrefixRecipe.Clear();
        ShimmerTransformLocks.Clear();
    }

    #region Detours
    private static System.Boolean Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
        return ItemIdHasPrefixRecipe.Contains(self.type) || orig(self);
    }
    #endregion
}