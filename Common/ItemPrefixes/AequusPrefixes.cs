using Aequus.Content.Items.Weapons.Classless;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.ItemPrefixes;

public class AequusPrefixes : ILoadable {
    public void Load(Mod mod) {
        DetourHelper.AddHook(typeof(PrefixLoader).GetMethod(nameof(PrefixLoader.CanRoll)), typeof(AequusPrefixes).GetMethod(nameof(On_PrefixLoader_CanRoll), BindingFlags.NonPublic | BindingFlags.Static));
    }

    public void Unload() {
    }

    #region Hooks
    private static bool On_PrefixLoader_CanRoll(Func<Item, int, bool> orig, Item item, int prefix) {
        if (item.ModItem is ClasslessWeapon && prefix < PrefixID.Count) {
            return true;
        }
        return orig(item, prefix);
    }
    #endregion
}