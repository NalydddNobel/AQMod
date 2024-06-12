using Aequus.Content.Items.Tools.Keychain;
using Aequus.Content.Tiles.CraftingStations.TrashCompactor;
using Aequus.DataSets.Structures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Aequus.DataSets;

public class ItemDataSet : DataSet {
    /// <summary>Items in this set ignore element inheritence from recipe ingredients.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> NoRecipeElementInheritence { get; private set; } = [];

    /// <summary>Items in this set ignore element inheritence from enemies which drop this item.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> NoNPCElementInheritence { get; private set; } = [
        ItemID.Gel,
        ItemID.Hook,
        ItemID.SharkFin,
        ItemID.SlimeStaff,
    ];

    /// <summary>Items in this set are unable to be submitted to the Physicist.</summary>
    [JsonProperty]
    public static Dictionary<IDEntry<ItemID>, Keychain.KeychainInfo> KeychainData { get; private set; } = [];
    /// <summary>Items in this set are unable to be submitted to the Physicist.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> CannotTradeWithPhysicist { get; private set; } = new();
    /// <summary>Items in this set are fishing junk.</summary>
    [JsonProperty]
    public static List<IDEntry<ItemID>> FishingJunk { get; private set; }
    /// <summary>Items in this set are potions.</summary>
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> Potions { get; private set; } = new();
    [JsonProperty]
    public static List<IDEntry<ItemID>> PillarFragmentsByColor { get; private set; } = new();

    /// <summary>
    /// A few item results which are ignored by the Shimmer Monocle, so that common transmuations don't have crappy bloated tooltips.
    /// </summary>
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> ShimmerTooltipResultIgnore { get; private set; } = new();
    /// <summary>
    /// Items marked as important will have special properties:
    /// <list type="bullet">
    /// <item>If this item is inside a chest, the chest will not gain Aequus loot, or Hardmode Chest Loot.</item>
    /// </list>
    /// </summary>
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> ImportantItem { get; private set; } = new();
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> CannotRename { get; private set; } = new();
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> IsDungeonLockBox { get; private set; } = new();
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> IsHellLockBox { get; private set; } = new();
    [JsonProperty]
    public static HashSet<IDEntry<ItemID>> IsStaffOfRegrowth { get; private set; } = new() {
        ItemID.StaffofRegrowth,
        ItemID.AcornAxe,
    };

    [JsonProperty]
    public static Dictionary<IDEntry<ItemID>, IDEntry<ProjectileID>> AmmoIdToProjectileId { get; private set; } = new();

    [JsonIgnore]
    public static Dictionary<int, TrashCompactorRecipe> CustomTrashCompactorRecipes { get; private set; } = new();

    public override void PostSetupContent() {
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

        foreach (int potion in Potions.Where(e => e.ValidEntry)) {
            ItemSets.CanGetPrefixes[potion] = true;
        }

        static void CheckPotion(Item item) {
            if (item.buffTime > 0 && item.buffType > 0 && item.consumable && item.maxStack >= 30 && item.damage <= 0 && item.healLife <= 0 && item.healMana <= 0 && !Main.persistentBuff[item.buffType] && !BuffSets.IsAFlaskBuff[item.buffType] && !ItemSets.IsFood[item.type] && !BuffSets.IsFedState[item.buffType]) {
                Potions.Add(item.type);
            }
        }

        static void CheckAmmo(Item item) {
            if (item.ammo <= ProjectileID.None || item.notAmmo || AmmoIdToProjectileId.ContainsKey(item.type)) {
                return;
            }

            // Custom case because rockets are weird.
            if (item.ammo == AmmoID.Rocket) {
                if (ProjectileID.Search.TryGetId(ItemID.Search.GetName(item.type), out int rocketID)) {
                    AmmoIdToProjectileId.Add(item.type, rocketID);
                }
                else if (ProjectileID.Search.TryGetId(ItemID.Search.GetName(item.type) + "Proj", out int rocketID2)) {
                    AmmoIdToProjectileId.Add(item.type, rocketID2);
                }
                else {
                    AmmoIdToProjectileId.Add(item.type, ProjectileID.RocketI);
                }
            }
            // ...and solutions...
            else if (item.ammo == AmmoID.Solution) {
                if (ProjectileID.Search.TryGetId(ItemID.Search.GetName(item.type), out int solutionId)) {
                    AmmoIdToProjectileId.Add(item.type, solutionId);
                }
                else if (ProjectileID.Search.TryGetId(ItemID.Search.GetName(item.type).Replace("Solution", "Spray"), out int solutionId2)) {
                    AmmoIdToProjectileId.Add(item.type, solutionId2);
                }
                else {
                    AmmoIdToProjectileId.Add(item.type, ProjectileID.RocketI);
                }
            }
            else {
                AmmoIdToProjectileId.Add(item.type, item.shoot);
            }
        }
    }

    public override void AddRecipes() {
    }
}