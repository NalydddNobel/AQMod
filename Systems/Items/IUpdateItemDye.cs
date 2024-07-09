namespace AequusRemake.Systems.Items;

public interface IUpdateItemDye {
    /// <param name="player"></param>
    /// <param name="isNotInVanitySlot"></param>
    /// <param name="isSetToHidden"></param>
    /// <param name="armorItem">The armor item.</param>
    /// <param name="dyeItem">The dye item.</param>
    void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem);
}