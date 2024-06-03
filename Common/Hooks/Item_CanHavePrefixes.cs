using Aequus.DataSets;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static bool On_Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
        if (ItemDataSet.Potions.Contains(self.type)) {
            return true;
        }

        return orig(self);
    }
}
