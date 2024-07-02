namespace Aequus.Core.Entities.Items.Components;

internal interface IModifyFishingPower {
    void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel);
}