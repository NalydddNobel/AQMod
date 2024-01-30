namespace Aequus.Common.Items.Components;

public interface IUpdateItemDye {
    void UpdateItemDye(Player player, System.Boolean isNotInVanitySlot, System.Boolean isSetToHidden, Item armorItem, Item dyeItem);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
    /// <param name="isNotInVanitySlot"></param>
    /// <param name="isSetToHidden"></param>
    /// <param name="armorItem">If you are an equipped item, this is you.</param>
    /// <param name="dyeItem">If you are a dye, this is you.</param>
    internal static void Player_UpdateItemDye(On_Player.orig_UpdateItemDye orig, Player self, System.Boolean isNotInVanitySlot, System.Boolean isSetToHidden, Item armorItem, Item dyeItem) {
        orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
        if (armorItem.ModItem is IUpdateItemDye armorDye) {
            armorDye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
        }
        if (dyeItem.ModItem is IUpdateItemDye dye) {
            dye.UpdateItemDye(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
        }
    }
}