using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;
using Terraria.ID;

namespace Aequus.Content.DataSets;

public class ItemSets : DataSet {
    /// <summary>
    /// A few item results which are ignored by the Shimmer Monocle, so that common transmuations don't have crappy bloated tooltips.
    /// </summary>
    [JsonProperty]
    public static HashSet<ItemEntry> ShimmerTooltipResultIgnore { get; private set; } = new();
    /// <summary>
    /// Contains item ids which are classified as 'Health Pickups'.
    /// </summary>
    [JsonProperty]
    public static HashSet<ItemEntry> IsHealthPickup { get; private set; } = new();
    /// <summary>
    /// Contains item ids which are classified as 'Mana Pickups'.
    /// </summary>
    [JsonProperty]
    public static HashSet<ItemEntry> IsManaPickup { get; private set; } = new();
    /// <summary>
    /// Contains items ids which are classified as 'Pickups', which are generally items which do not appear in your inventory. Instead they grant some sort of effect.
    /// <para>This set includes data from <see cref="IsHealthPickup"/> and <see cref="IsManaPickup"/>.</para>
    /// </summary>
    [JsonProperty]
    public static HashSet<ItemEntry> IsPickup { get; private set; } = new();
    /// <summary>
    /// Items marked as important will have special properties:
    /// <list type="bullet">
    /// <item>If this item is inside a chest, the chest will not gain Aequus loot, or Hardmode Chest Loot.</item>
    /// </list>
    /// </summary>
    [JsonProperty]
    public static HashSet<ItemEntry> ImportantItem { get; private set; } = new();
    [JsonProperty]
    public static HashSet<ItemEntry> CannotRename { get; private set; } = new();
    [JsonProperty]
    public static List<ItemEntry> CelestialFragmentsByColor { get; private set; } = new();
    [JsonProperty]
    public static HashSet<ItemEntry> IsDungeonLockBox { get; private set; } = new();
    [JsonProperty]
    public static HashSet<ItemEntry> IsHellLockBox { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<ItemEntry, ProjectileEntry> AmmoIdToProjectileId { get; private set; }

    [JsonIgnore]
    public static Dictionary<int, TrashCompactorRecipe> CustomTrashCompactorRecipes { get; private set; } = new();

    public override void AddRecipes() {
        foreach (var item in ContentSamples.ItemsByType.Values) {
            if (item == null || item.ammo <= ProjectileID.None || item.notAmmo || AmmoIdToProjectileId.ContainsKey(item.type)) {
                continue;
            }

            // Custom case because rockets are weird.
            if (item.ammo == AmmoID.Rocket) {
                if (ProjectileID.Search.TryGetId(ItemID.Search.GetName(item.type), out int rocketID)) {
                    AmmoIdToProjectileId.Add((ItemEntry)item.type, (ProjectileEntry)rocketID);
                }
                else {
                    AmmoIdToProjectileId.Add((ItemEntry)item.type, (ProjectileEntry)ProjectileID.RocketI);
                }
                continue;
            }

            AmmoIdToProjectileId.Add((ItemEntry)item.type, (ProjectileEntry)item.shoot);
        }

        IsPickup.AddRange(IsHealthPickup);
        IsPickup.AddRange(IsManaPickup);
    }
}