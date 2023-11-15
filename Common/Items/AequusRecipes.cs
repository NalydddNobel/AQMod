using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public sealed partial class AequusRecipes : ModSystem {
    public static readonly HashSet<int> ItemIdHasPrefixRecipe = new();
    public static readonly Dictionary<int, Condition> ShimmerTransformLocks = new();

    public override void Load() {
        On_Item.CanHavePrefixes += Item_CanHavePrefixes;
    }

    public override void Unload() {
        ItemIdHasPrefixRecipe.Clear();
        ShimmerTransformLocks.Clear();
    }

    #region Detours
    private static bool Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
        return ItemIdHasPrefixRecipe.Contains(self.type) || orig(self);
    }
    #endregion
}