using Aequus.Common.Items.Components;
using System.Collections.Generic;
using Terraria.GameContent.Creative;
using CreativeItemGroup = Terraria.ID.ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup;

namespace Aequus.Common.JourneyMode;

public class JourneyModeSystem : ModSystem {
    internal static readonly Dictionary<int, FilterFullfillment> _filterOverrideCache = new();

    public override void Load() {
        On_ItemFilters.Tools.FitsFilter += FilterOverride_Tools;
        On_ContentSamples.CreativeHelper.SetCreativeMenuOrder += FinalSortingAdjustments;
    }

    public override void Unload() {
        _filterOverrideCache.Clear();
    }

    private bool FilterOverride_Tools(On_ItemFilters.Tools.orig_FitsFilter orig, ItemFilters.Tools self, Item entry) {
        return orig(self, entry) ? true :
            _filterOverrideCache.TryGetValue(entry.type, out FilterFullfillment filter) && filter == FilterFullfillment.Tools;
    }

    private static void FinalSortingAdjustments(On_ContentSamples.CreativeHelper.orig_SetCreativeMenuOrder orig) {
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
