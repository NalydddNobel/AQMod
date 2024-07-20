namespace Aequus;
public partial class AequusItem {
    private void Load_Prefixes() {
        On_Item.CanHavePrefixes += On_Item_CanHavePrefixes;
        On_Item.CanApplyPrefix += On_Item_CanApplyPrefix;
    }

    private bool On_Item_CanApplyPrefix(On_Item.orig_CanApplyPrefix orig, Item self, int prefix) {
        if (self.IsArmor()) {
            if (prefix <= 0) {
                return false;
            }

            return true;
        }
        return orig(self, prefix);
    }

    private bool On_Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
        return orig(self) || self.IsArmor();
    }
}