using Aequus.Common.Items.Components;
using Aequus.Content.DataSets;
using Aequus.Content.Weapons.Classless;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.Utilities;

namespace Aequus.Common.ItemPrefixes;

public class AequusPrefixes : GlobalItem {
    public static List<CooldownPrefix> RegisteredCooldownPrefixes { get; private set; } = new();

    public override void Load() {
        On_Item.CanHavePrefixes += On_Item_CanHavePrefixes;
        HookManager.ApplyAndCacheHook(typeof(PrefixLoader).GetMethod(nameof(PrefixLoader.CanRoll)), typeof(AequusPrefixes).GetMethod(nameof(On_PrefixLoader_CanRoll), BindingFlags.NonPublic | BindingFlags.Static));
    }

    public override void Unload() {
        RegisteredCooldownPrefixes?.Clear();
    }

    #region Hooks
    private static bool On_Item_CanHavePrefixes(On_Item.orig_CanHavePrefixes orig, Item self) {
        if (ItemMetadata.Potions.Contains(self.type)) {
            return true;
        }

        return orig(self);
    }

    private static bool On_PrefixLoader_CanRoll(Func<Item, int, bool> orig, Item item, int prefix) {
        if (item.ModItem is ClasslessWeapon && prefix < PrefixID.Count) {
            return true;
        }
        return orig(item, prefix);
    }
    #endregion

    public override int ChoosePrefix(Item item, UnifiedRandom rand) {
        if (item.ModItem is ICooldownItem && rand.NextBool(4)) {
            return rand.Next(RegisteredCooldownPrefixes).Type;
        }
        return -1;
    }
}