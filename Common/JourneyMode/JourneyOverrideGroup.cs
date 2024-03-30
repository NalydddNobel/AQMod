using System.Collections.Generic;

namespace Aequus.Common.JourneyMode;

internal readonly record struct JourneyOverrideGroup(ContentSamples.CreativeHelper.ItemGroup ItemGroupOverride) : IJourneySortOverrideProvider {
    public ContentSamples.CreativeHelper.ItemGroup? ProvideItemGroup() {
        return ItemGroupOverride;
    }

    public int? ProvideItemGroupOrdering(ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary) {
        return null;
    }
}
