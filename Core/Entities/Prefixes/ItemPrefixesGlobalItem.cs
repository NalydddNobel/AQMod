using Aequu2.Content.Dedicated.BeyondCoin;
using Aequu2.Content.Items.PermaPowerups.Shimmer;

namespace Aequu2.Core.Entities.Prefixes;

public class ItemPrefixesGlobalItem : GlobalItem {
    private static int _loops;

    public override void Load() {
        On_Item.Prefix += RerollPrefixes;
    }

    private static bool RerollPrefixes(On_Item.orig_Prefix orig, Item item, int pre) {
        if (pre != -2 || _loops != 0 || !AequusSystem.usedReforgeBook) {
            return orig(item, pre);
        }

        int rerolls = TinkerersGuidebook.BonusRerolls;
        ShimmerCoinPlayer.TinkererRerolls(ref rerolls);

        bool successfullyAppliedAnyPrefix = false;
        int finalPrefix = 0;
        int value = 0;
        Item cloneItem = new Item();

        for (int i = 0; i < rerolls; i++) {
            _loops = i + 1;
            cloneItem.ResetPrefix();
            successfullyAppliedAnyPrefix |= cloneItem.Prefix(pre);
            int prefixValue = cloneItem.value / 5;
            if (cloneItem.prefix == PrefixID.Ruthless) {
                prefixValue = (int)(prefixValue * 2.15f);
            }
            if ((cloneItem.pick > 0 || cloneItem.axe > 0 || cloneItem.hammer > 0) && cloneItem.prefix == PrefixID.Light) {
                prefixValue = (int)(prefixValue * 2.5f);
            }
            if (prefixValue > value || finalPrefix == 0) {
                finalPrefix = cloneItem.prefix;
                value = prefixValue;
            }
        }

        _loops = 0;
        if (successfullyAppliedAnyPrefix && finalPrefix > 0) {
            pre = finalPrefix;
        }

        return orig(item, pre);
    }
}
