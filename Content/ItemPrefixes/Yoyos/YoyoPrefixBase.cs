using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Yoyos;

public abstract class YoyoPrefixBase : AequusPrefix {
    public override PrefixCategory Category => PrefixCategory.AnyWeapon;

    public override bool CanRoll(Item item) {
        return ItemID.Sets.Yoyo[item.type];
    }

    public override void ModifyValue(ref float valueMult) {
        valueMult = 1.3f;
    }
}