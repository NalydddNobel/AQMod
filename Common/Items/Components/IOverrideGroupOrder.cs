using System.Collections.Generic;

namespace Aequus.Common.Items.Components;

internal interface IOverrideGroupOrder {
    void ModifyItemGroup(ref ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary);
}
