using System.Collections.Generic;
using Terraria.GameContent.Items;

namespace Aequu2.Old.Common.Items.Variants;

public class Aequu2ItemVariants : ILoad {
    private static readonly Dictionary<int, List<ItemVariants.VariantEntry>> CustomAequu2Variants = new();

    private static void Clear() {
        if (CustomAequu2Variants.Count == 0) {
            return;
        }

        foreach (var entry in CustomAequu2Variants.Values) {
            entry?.Clear();
        }
        CustomAequu2Variants.Clear();
    }

    public static void AddVariant(int itemId, ItemVariant variant, params Condition[] conditions) {
        ItemVariants.VariantEntry entry = new(variant);
        if (CustomAequu2Variants.TryGetValue(itemId, out var customVariants) && customVariants != null) {
            customVariants.Add(entry);
        }
        else {
            CustomAequu2Variants[itemId] = new() { entry };
        }
        ((List<Condition>)entry.Conditions).AddRange(conditions);
    }

    public void Load(Mod mod) {
        Clear();
        AddVariant(ItemID.Kraken, ItemVariants.WeakerVariant, Condition.RemixWorld);
        On_ItemVariants.SelectVariant += On_ItemVariants_SelectVariant;
    }

    private ItemVariant On_ItemVariants_SelectVariant(On_ItemVariants.orig_SelectVariant orig, int itemId) {
        if (CustomAequu2Variants.TryGetValue(itemId, out var variant) && variant != null) {
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