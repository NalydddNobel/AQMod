using System.Collections.Generic;

namespace Aequus.Common.JourneyMode;

public interface IJourneySortOverrideProvider {
    ContentSamples.CreativeHelper.ItemGroup? ProvideItemGroup();
    int? ProvideItemGroupOrdering(ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary);
}
