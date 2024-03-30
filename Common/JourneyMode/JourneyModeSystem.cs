using Aequus.Common.Items.Components;
using System.Collections.Generic;
using CreativeItemGroup = Terraria.ID.ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup;

namespace Aequus.Common.JourneyMode;

public class JourneyModeSystem : ModSystem {
    public override void Load() {
        On_ContentSamples.CreativeHelper.SetCreativeMenuOrder += CreativeHelper_SetCreativeMenuOrder;
    }

    private static void CreativeHelper_SetCreativeMenuOrder(On_ContentSamples.CreativeHelper.orig_SetCreativeMenuOrder orig) {
        orig();
        Mod compareMod = Aequus.Instance;
        Dictionary<int, CreativeItemGroup> originalSortingIds = ContentSamples.ItemCreativeSortingId;
        Dictionary<int, CreativeItemGroup> copySortingIds = new Dictionary<int, CreativeItemGroup>(originalSortingIds);
        foreach (KeyValuePair<int, CreativeItemGroup> pair in copySortingIds) {
            if (ItemLoader.GetItem(pair.Key) is IOverrideGroupOrder overrideGroupOrder) {
                CreativeItemGroup value = pair.Value;
                overrideGroupOrder.ModifyItemGroup(ref value, originalSortingIds);

                originalSortingIds[pair.Key] = value;
            }
        }
    }

    private static void FinalSortingAdjustments(On_ContentSamples.orig_RebuildItemCreativeSortingIDsAfterRecipesAreSetUp orig) {
        orig();
        Mod compareMod = Aequus.Instance;
        Dictionary<int, CreativeItemGroup> originalSortingIds = ContentSamples.ItemCreativeSortingId;
        Dictionary<int, CreativeItemGroup> copySortingIds = new Dictionary<int, CreativeItemGroup>(originalSortingIds);
        foreach (KeyValuePair<int, CreativeItemGroup> pair in copySortingIds) {
            if (ItemLoader.GetItem(pair.Key) is IOverrideGroupOrder overrideGroupOrder) {
                CreativeItemGroup value = pair.Value;
                overrideGroupOrder.ModifyItemGroup(ref value, originalSortingIds);

                originalSortingIds[pair.Key] = value;
            }
        }
    }
}
