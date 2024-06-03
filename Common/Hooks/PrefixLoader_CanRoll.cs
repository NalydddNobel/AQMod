using Aequus.Common.Items.Components;
using System;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static bool On_PrefixLoader_CanRoll(Func<Item, int, bool> orig, Item item, int prefix) {
        if (item.ModItem is IOmniclassItem && prefix < PrefixID.Count) {
            return true;
        }

        return orig(item, prefix);
    }
}
