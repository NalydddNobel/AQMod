using Aequus.Common;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;

namespace Aequus.Systems.Shimmer;

public class Shimmer : LoadedType {
    private readonly Dictionary<int, List<IShimmerOverride>> _byTypeOverride = [];
    private readonly List<IShimmerOverride> _globalOverrides = [];

    protected override void Load() {
        On_ShimmerTransforms.IsItemTransformLocked += On_ShimmerTransforms_IsItemTransformLocked;
        On_Item.CanShimmer += On_Item_CanShimmer;
        On_Item.GetShimmered += On_Item_GetShimmered;
    }


    #region Hooks
    private bool On_ShimmerTransforms_IsItemTransformLocked(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type) {
        return ModContent.GetInstance<Shimmer>().IsItemTransformLocked(type) ?? orig(type);
    }

    private static bool On_Item_CanShimmer(On_Item.orig_CanShimmer orig, Item item) {
        return ModContent.GetInstance<Shimmer>().CanShimmer(item) ?? orig(item);
    }

    private static void On_Item_GetShimmered(On_Item.orig_GetShimmered orig, Item self) {
        if (ModContent.GetInstance<Shimmer>().OverrideTransformation(self)) {
            return;
        }

        orig(self);
    }
    #endregion

    public void AddShimmerOverride(int type, IShimmerOverride effect) {
        (CollectionsMarshal.GetValueRefOrAddDefault(_byTypeOverride, type, out bool _) ??= []).Add(effect);
    }

    public void AddGlobalShimmerOverride(IShimmerOverride effect) {
        _globalOverrides.Add(effect);
    }

    /// <summary>Emulates all vanilla shimmer on-transmutation effects. Including getting the achievement, the sound, particles, networking, ect.</summary>
    /// <param name="item"></param>
    public static void GetShimmered(Item item) {
        item.shimmerTime = item.stack > 0 ? 1f : 0f;

        item.shimmerWet = true;
        item.wet = true;
        item.velocity *= 0.1f;

        if (Main.netMode == NetmodeID.SinglePlayer) {
            Item.ShimmerEffect(item.Center);
        }
        else {
            NetMessage.SendData(146, -1, -1, null, 0, (int)item.Center.X, (int)item.Center.Y);
            NetMessage.SendData(145, -1, -1, null, item.whoAmI, 1f);
        }

        AchievementsHelper.NotifyProgressionEvent(27);
    }

    public static void CreateShimmerLoop(params int[] items) {
        for (int i = 0; i < items.Length; i++) {
            SetTransformation(items[i], items[(i + 1) % items.Length]);
        }
    }

    /// <summary>Attempts to safely set the shimmer result of this item, by inserting it to the end of existing chains or loops.</summary>
    /// <param name="from"></param>
    /// <param name="into"></param>
    public static void SetTransformation(int from, int into) {
        if (ItemID.Sets.ShimmerCountsAsItem[from] > -1) {
            from = ItemID.Sets.ShimmerCountsAsItem[from];
        }

        // Abort fancy logic if it already converts into the item, or is an invalid Id (0 or larger than ItemLoader.ItemCount)
        if (ItemID.Sets.ShimmerTransformToItem[from] == into || ItemID.Sets.ShimmerTransformToItem[from] <= 0 || ItemID.Sets.ShimmerTransformToItem[from] >= ItemLoader.ItemCount) {
            ItemID.Sets.ShimmerTransformToItem[from] = into;
            return;
        }

        // Get the endpoint of the shimmer tree we are inserting into.
        int fromEndpoint = GetShimmerTreeEndpoint(from, out _);

        // Now check if the item we are linking onto is part of a tree.
        if (ItemID.Sets.ShimmerTransformToItem[into] > 0 && ItemID.Sets.ShimmerTransformToItem[into] < ItemLoader.ItemCount) {
            // If so, get the endpoint of that item's shimmer tree.
            int intoEndpoint = GetShimmerTreeEndpoint(into, out _);

            // If the endpoint is also the same as the from item, don't set it
            // (ItemID.Sets.ShimmerTransformToItem[from] = from;)
            if (intoEndpoint != from) {
                ItemID.Sets.ShimmerTransformToItem[intoEndpoint] = from;
            }
        }

        // If the endpoint is also the same as the into item, don't set it
        // (ItemID.Sets.ShimmerTransformToItem[into] = into;)
        if (fromEndpoint != into) {
            ItemID.Sets.ShimmerTransformToItem[fromEndpoint] = into;
        }
    }

    public static int GetShimmerTreeEndpoint(int start, out ShimmerTransmutationTreeType type) {
        int lastType = start;
        for (int i = 0; i < ItemLoader.ItemCount; i++) {
            // Reached an endpoint where the item has no further conversions.
            if (ItemID.Sets.ShimmerTransformToItem[lastType] <= 0) {
                type = ShimmerTransmutationTreeType.Line;
                return lastType;
            }

            // Reached an endpoint where the item returns to the starting item.
            if (ItemID.Sets.ShimmerTransformToItem[lastType] == start) {
                type = ShimmerTransmutationTreeType.Loop;
                return lastType;
            }

            lastType = ItemID.Sets.ShimmerTransformToItem[lastType];
            if (ItemID.Sets.ShimmerCountsAsItem[lastType] > -1) {
                lastType = ItemID.Sets.ShimmerCountsAsItem[lastType];
            }
        }

        // Reached a broken endpoint where it must loop, but never reach the starting item.
        type = ShimmerTransmutationTreeType.Broken;
        return lastType;
    }

    internal bool? IsItemTransformLocked(int type) {
        type = OverrideShimmerType(type);
        foreach (IShimmerOverride o in GetAllOverrides(type)) {
            bool? result = o.IsTransformLocked(type);
            if (result.HasValue) {
                return result.Value;
            }
        }

        return null;
    }

    internal bool? CanShimmer(Item item) {
        int type = OverrideShimmerType(item.type);
        foreach (IShimmerOverride o in GetAllOverrides(type, item.ModItem)) {
            bool? result = o.CanShimmer(item, type);
            if (result.HasValue) {
                return result.Value;
            }
        }

        return null;
    }

    internal bool OverrideTransformation(Item item) {
        int type = OverrideShimmerType(item.type);
        foreach (IShimmerOverride o in GetAllOverrides(type, item.ModItem)) {
            if (o?.GetShimmered(item, type) == true) {
                return true;
            }
        }

        return false;
    }

    private IEnumerable<IShimmerOverride> GetAllOverrides(int type, ModItem? modItem = null) {
        modItem ??= ItemLoader.GetItem(type);
        if (modItem is IShimmerOverride modItemShimmerOverride) {
            yield return modItemShimmerOverride;
        }

        if (_byTypeOverride.TryGetValue(type, out List<IShimmerOverride>? values)) {
            for (int i = 0; i < values.Count; i++) {
                yield return values[i];
            }
        }

        for (int i = 0; i < _globalOverrides.Count; i++) {
            yield return _globalOverrides[i];
        }
    }

    private static int OverrideShimmerType(int type) {
        return ItemID.Sets.ShimmerCountsAsItem[type] > -1 ? ItemID.Sets.ShimmerCountsAsItem[type] : type;
    }
}
