using Aequus.Core.DataSets;
using Newtonsoft.Json;
using Terraria.ID;

namespace Aequus.Content.DataSets;

[DataID(typeof(ItemID))]
public class ItemSets : DataSet {
    public ItemSets() : base() {
    }

    /// <summary>
    /// A few item results which are ignored by the Shimmer Monocle, so that common transmuations don't have crappy bloated tooltips.
    /// </summary>
    [JsonProperty]
    public static IDSetValue ShimmerTooltipResultIgnore;
    /// <summary>
    /// Contains item ids which are classified as 'Health Pickups'.
    /// </summary>
    [JsonProperty]
    public static IDSetValue IsHealthPickup;
    /// <summary>
    /// Contains item ids which are classified as 'Mana Pickups'.
    /// </summary>
    [JsonProperty]
    public static IDSetValue IsManaPickup;
    /// <summary>
    /// Contains items ids which are classified as 'Pickups', which are generally items which do not appear in your inventory. Instead they grant some sort of effect.
    /// <para>This set includes data from <see cref="IsHealthPickup"/> and <see cref="IsManaPickup"/>.</para>
    /// </summary>
    [JsonProperty]
    public static IDSetValue IsPickup;
    /// <summary>
    /// Items marked as important will have special properties:
    /// <list type="bullet">
    /// <item>If this item is inside a chest, the chest will not gain Aequus loot, or Hardmode Chest Loot.</item>
    /// </list>
    /// </summary>
    [JsonProperty]
    public static IDSetValue ImportantItem;
    [JsonProperty]
    public static IDSetValue CannotRename;
    [JsonProperty]
    public static IDSetValue CelestialFragmentsByColor;

    public override void AddRecipes() {
        IsPickup.AddRange(IsHealthPickup);
        IsPickup.AddRange(IsManaPickup);
    }
}