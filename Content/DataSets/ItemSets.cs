using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.Core.DataSets;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aequus.Content.DataSets;

public class ItemSets : DataSet {
    /// <summary>
    /// Items in this set are potions.
    /// </summary>
    [JsonProperty]
    public static List<ItemEntry> Potions { get; private set; } = new();
    [JsonProperty]
    public static List<ItemEntry> PillarFragmentsByColor { get; private set; } = new();

    /// <summary>
    /// A few item results which are ignored by the Shimmer Monocle, so that common transmuations don't have crappy bloated tooltips.
    /// </summary>
    [JsonProperty]
    public static HashSet<ItemEntry> ShimmerTooltipResultIgnore { get; private set; } = new();
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
    public static HashSet<ItemEntry> IsDungeonLockBox { get; private set; } = new();
    [JsonProperty]
    public static HashSet<ItemEntry> IsHellLockBox { get; private set; } = new();

    [JsonProperty]
    public static Dictionary<ItemEntry, ProjectileEntry> AmmoIdToProjectileId { get; private set; }

    [JsonIgnore]
    public static Dictionary<int, TrashCompactorRecipe> CustomTrashCompactorRecipes { get; private set; } = new();

    public override void AddRecipes() {
        int start = 0;
        int end = ItemLoader.ItemCount;
        for (int i = start; i < end; i++) {
            Item item = new Item(i);
            if (item == null) {
                continue;
            }

            CheckPotion(item);
            CheckAmmo(item);
        }

        static void CheckPotion(Item item) {
            if (item.buffTime > 0 && item.buffType > 0 && item.consumable && item.maxStack >= 30 && item.damage <= 0 && item.healLife <= 0 && item.healMana <= 0 && !Main.persistentBuff[item.buffType] && !BuffID.Sets.IsAFlaskBuff[item.buffType] && !ItemID.Sets.IsFood[item.type] && !BuffID.Sets.IsFedState[item.buffType]) {
                Potions.Add((ItemEntry)item.type);
            }
        }
        static void CheckAmmo(Item item) {
            if (item.ammo <= ProjectileID.None || item.notAmmo || AmmoIdToProjectileId.ContainsKey(item.type)) {
                return;
            }

            // Custom case because rockets are weird.
            if (item.ammo == AmmoID.Rocket) {
                if (ProjectileID.Search.TryGetId(ItemID.Search.GetName(item.type), out int rocketID)) {
                    AmmoIdToProjectileId.Add((ItemEntry)item.type, (ProjectileEntry)rocketID);
                }
                else {
                    AmmoIdToProjectileId.Add((ItemEntry)item.type, (ProjectileEntry)ProjectileID.RocketI);
                }
            }
            else {
                AmmoIdToProjectileId.Add((ItemEntry)item.type, (ProjectileEntry)item.shoot);
            }
        }
    }
}