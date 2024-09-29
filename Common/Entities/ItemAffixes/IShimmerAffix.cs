using Aequus.Systems.Shimmer;

namespace Aequus.Common.Entities.ItemAffixes;

public interface IShimmerAffix {
    bool OnShimmer(Item item);

    public static void ClearPrefixOnShimmer(Item item) {
        int oldStack = item.stack;
        item.SetDefaults(item.netID);
        item.stack = oldStack;
        item.prefix = 0;
        item.shimmered = true;

        Shimmer.GetShimmered(item);
    }
}
