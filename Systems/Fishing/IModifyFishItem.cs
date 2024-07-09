namespace AequusRemake.Systems.Fishing;

/// <summary>Only works on Fishing Pole or Bait items.</summary>
internal interface IModifyFishItem {
    void ModifyFishItem(Player player, Item fish);
}
