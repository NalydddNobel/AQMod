using AequusRemake.Systems.Items;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    internal static void Player_UpdateItemDye(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
        orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

        if (armorItem.ModItem is IUpdateItemDye armorDye) {
            armorDye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
        }

        if (dyeItem.ModItem is IUpdateItemDye dye) {
            dye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
        }
    }
}
