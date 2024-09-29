using Aequus.Common.Entities.ItemAffixes;
using Aequus.Common.Items.Dedications;
using Aequus.Common.Recipes;
using Aequus.Content.Dedicated;
using Terraria.GameContent;

namespace Aequus;
public partial class AequusItem {
    public void Load_Shimmer() {
        On_Item.GetShimmered += On_Item_GetShimmered;
        On_ShimmerTransforms.IsItemTransformLocked += On_ShimmerTransforms_IsItemTransformLocked;
        On_Item.CanShimmer += On_Item_CanShimmer;
    }

    private static bool On_Item_CanShimmer(On_Item.orig_CanShimmer orig, Item item) {
        if (DedicationRegistry.Contains(item.type)) {
            return true;
        }

        return orig(item);
    }

    private static bool On_ShimmerTransforms_IsItemTransformLocked(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type) {
        if (AequusRecipes.Overrides.ShimmerTransformLocks.TryGetValue(type, out var condition)) {
            return !condition.IsMet();
        }

        return orig(type);
    }

    private static void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item item) {
        if (PrefixLoader.GetPrefix(item.prefix) is IShimmerAffix affix && affix.OnShimmer(item)) {
            return;
        }

        DedicatedFaeling.SpawnFaelingsFromShimmer(item);

        if (item.stack <= 0) {
            return;
        }

        orig(item);
    }
}