namespace AequusRemake.Systems.Fishing;

internal interface IModifyFishingPower {
    void ModifyFishingPower(Player player, Item fishingRod, ref float fishingLevel);
}