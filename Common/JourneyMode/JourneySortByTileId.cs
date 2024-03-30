using System.Collections.Generic;

namespace Aequus.Common.JourneyMode;

public readonly record struct JourneySortByTileId(int TileId, ContentSamples.CreativeHelper.ItemGroup? ItemGroupOverride = null) : IJourneySortOverrideProvider {
    public ContentSamples.CreativeHelper.ItemGroup? ProvideItemGroup() {
        return ItemGroupOverride;
    }

    public int? ProvideItemGroupOrdering(ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary) {
        foreach (var pair in groupDictionary) {
            if (ContentSamples.ItemsByType[pair.Key].createTile == TileId) {
                return pair.Value.OrderInGroup;
            }
        }

        return null;
    }
}
