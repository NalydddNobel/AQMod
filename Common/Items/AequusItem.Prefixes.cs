using Aequus.Items.Weapons.Classless;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

public partial class AequusItem : GlobalItem {
    private static bool On_PrefixLoader_CanRoll(Func<Item, int, bool> orig, Item item, int prefix) {
        if (item.ModItem is ClasslessWeapon && prefix < PrefixID.Count) {
            return true;
        }
        return orig(item, prefix);
    }
}