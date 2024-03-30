using Aequus.Core.Initialization;
using System;

namespace Aequus.Common.JourneyMode;

[AttributeUsage(AttributeTargets.Class)]
internal class FilterOverrideAttribute : AutoloadXAttribute {
    public FilterFullfillment _filter;

    public FilterOverrideAttribute(FilterFullfillment filter) {
        _filter = filter;
    }

    internal override void Load(ModType modType) {
        if (modType is not ModItem modItem) {
            throw new Exception($"Attribute {nameof(FilterOverrideAttribute)} can only be used on {nameof(ModItem)}s.");
        }

        JourneyModeSystem._filterOverrideCache.Add(modItem.Type, _filter);
    }
}

internal enum FilterFullfillment {
    Tools
}
