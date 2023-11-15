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
    public static DataIDValueSet ShimmerTooltipResultIgnore;
    /// <summary>
    /// Contains item ids which are classified as 'Health Pickups'.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet IsHealthPickup;
    /// <summary>
    /// Contains item ids which are classified as 'Mana Pickups'.
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet IsManaPickup;
    /// <summary>
    /// Contains items ids which are classified as 'Pickups', which are generally items which do not appear in your inventory. Instead they grant some sort of effect.
    /// <para>This set includes data from <see cref="IsHealthPickup"/> and <see cref="IsManaPickup"/>.</para>
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet IsPickup;
    /// <summary>
    /// Items marked as important will have special properties:
    /// <list type="bullet">
    /// <item>If this item is inside a chest, the chest will not gain Aequus loot, or Hardmode Chest Loot.</item>
    /// </list>
    /// </summary>
    [JsonProperty]
    public static DataIDValueSet ImportantItem;
    [JsonProperty]
    public static DataIDValueSet CannotRename;
    [JsonProperty]
    public static DataIDValueSet CelestialFragmentsByColor;
    [JsonProperty]
    public static DataIDValueSet IsDungeonLockBox;
    [JsonProperty]
    public static DataIDValueSet IsHellLockBox;

    [JsonProperty]
    [DataIDKeyValue(typeof(ItemID), typeof(ProjectileID))]
    public static DataIDKeyValueDictionary AmmoIdToProjectileId;

    public override void AddRecipes() {
        foreach (var item in ContentSamples.ItemsByType.Values) {
            if (item == null || item.ammo <= ProjectileID.None || item.notAmmo || AmmoIdToProjectileId.ContainsId(item.type)) {
                continue;
            }

            // Custom case because rockets are weird.
            if (item.ammo == AmmoID.Rocket) {
                if (ProjectileID.Search.TryGetId(ItemID.Search.GetName(item.type), out int rocketID)) {
                    AmmoIdToProjectileId.Add(item.type, rocketID);
                }
                else {
                    AmmoIdToProjectileId.Add(item.type, ProjectileID.RocketI);
                }
                continue;
            }

            AmmoIdToProjectileId.Add(item.type, item.shoot);
        }

        AmmoIdToProjectileId[ItemID.Gel] = ProjectileID.Flames;
        AmmoIdToProjectileId[ItemID.FallenStar] = ProjectileID.StarCannonStar;
        AmmoIdToProjectileId[ItemID.SandBlock] = ProjectileID.SandBallGun;
        AmmoIdToProjectileId[ItemID.EbonsandBlock] = ProjectileID.EbonsandBallFalling;
        AmmoIdToProjectileId[ItemID.CrimsandBlock] = ProjectileID.CrimsandBallFalling;
        AmmoIdToProjectileId[ItemID.PearlsandBlock] = ProjectileID.PearlSandBallFalling;
        AmmoIdToProjectileId[ItemID.GreenSolution] = ProjectileID.PureSpray;
        AmmoIdToProjectileId[ItemID.BlueSolution] = ProjectileID.HallowSpray;
        AmmoIdToProjectileId[ItemID.PurpleSolution] = ProjectileID.CorruptSpray;
        AmmoIdToProjectileId[ItemID.RedSolution] = ProjectileID.CrimsonSpray;
        AmmoIdToProjectileId[ItemID.DarkBlueSolution] = ProjectileID.MushroomSpray;
        AmmoIdToProjectileId[ItemID.DirtSolution] = ProjectileID.DirtSpray;
        AmmoIdToProjectileId[ItemID.SandSolution] = ProjectileID.SandSpray;
        AmmoIdToProjectileId[ItemID.SnowSolution] = ProjectileID.SnowSpray;
        AmmoIdToProjectileId[ItemID.MiniNukeI] = ProjectileID.MiniNukeRocketI;
        AmmoIdToProjectileId[ItemID.MiniNukeII] = ProjectileID.MiniNukeRocketII;

        IsPickup.AddRange(IsHealthPickup);
        IsPickup.AddRange(IsManaPickup);
    }
}