using AequusRemake.Core.Entities.Items.Components;
using System;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static bool On_PrefixLoader_CanRoll(Func<Item, int, bool> orig, Item item, int prefix) {
        if (item.ModItem is IOmniclassItem && prefix < PrefixID.Count) {
            return true;
        }

        return orig(item, prefix);
    }
}
