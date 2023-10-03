using System.Collections.Generic;
using Terraria.ID;

namespace Aequus.Common.DataSets;

public class ItemSets : DataSet {
    protected override ContentFileInfo ContentFileInfo => new(ItemID.Search);

    /// <summary>
    /// Contains item ids which are classified as 'Health Pickups'.
    /// </summary>
    public static readonly HashSet<int> IsHealthPickup = new();
    /// <summary>
    /// Contains item ids which are classified as 'Mana Pickups'.
    /// </summary>
    public static readonly HashSet<int> IsManaPickup = new();
    /// <summary>
    /// Contains items ids which are classified as 'Pickups', which are generally items which do not appear in your inventory. Instead they grant some sort of effect.
    /// <para>This set includes data from <see cref="IsHealthPickup"/> and <see cref="IsManaPickup"/>.</para>
    /// </summary>
    public static readonly HashSet<int> IsPickup = new();
    /// <summary>
    /// Items marked as important will have special properties:
    /// <list type="bullet">
    /// <item>If this item is inside a chest, the chest will not gain Aequus loot, or Hardmode Chest Loot.</item>
    /// </list>
    /// </summary>
    public static readonly HashSet<int> ImportantItem = new();
    public static readonly HashSet<int> CannotRename = new();
    public static readonly List<int> OrderedPillarFragments_ByClass = new();
    public static readonly List<int> OrderedPillarFragments_ByColor = new();

    public override void AddRecipes() {
        IsPickup.AddRange(IsHealthPickup);
        IsPickup.AddRange(IsManaPickup);
    }
}