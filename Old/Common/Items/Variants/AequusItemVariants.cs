using System.Collections.Generic;
using Terraria.GameContent.Items;

namespace Aequus.Old.Common.Items.Variants;

public class AequusItemVariants : ILoad {
    private static readonly Dictionary<int, List<ItemVariants.VariantEntry>> CustomAequusVariants = new();

    private static void Clear() {
        if (CustomAequusVariants.Count == 0) {
            return;
        }

        foreach (var entry in CustomAequusVariants.Values) {
            entry?.Clear();
        }
        CustomAequusVariants.Clear();
    }

    public static void AddVariant(int itemId, ItemVariant variant, params Condition[] conditions) {
        ItemVariants.VariantEntry entry = new(variant);
        if (CustomAequusVariants.TryGetValue(itemId, out var customVariants) && customVariants != null) {
            customVariants.Add(entry);
        }
        else {
            CustomAequusVariants[itemId] = new() { entry };
        }
        ((List<Condition>)entry.Conditions).AddRange(conditions);
    }

    public void Load(Mod mod) {
        Clear();
        AddVariant(ItemID.Kraken, ItemVariants.WeakerVariant, Condition.RemixWorld);
        On_ItemVariants.SelectVariant += On_ItemVariants_SelectVariant;
    }

    private ItemVariant On_ItemVariants_SelectVariant(On_ItemVariants.orig_SelectVariant orig, int itemId) {
        if (CustomAequusVariants.TryGetValue(itemId, out var variant) && variant != null) {
            foreach (var item in variant) {
                if (item.AnyConditionMet()) {
                    return item.Variant;
                }
            }
        }

        return orig(itemId);
    }

    public void Unload() {
        Clear();
    }
}