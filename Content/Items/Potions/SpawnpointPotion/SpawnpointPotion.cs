
namespace Aequus.Content.Items.Potions.SpawnpointPotion;

// TODO -- Add map icon for beacon potion portal.
public class SpawnpointPotion : ModItem {
    public static readonly int Cooldown = 600;

    public override void SetStaticDefaults() {
        Item.CloneResearchUnlockCount(ItemID.PotionOfReturn);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.PotionOfReturn);
    }

    public override bool? UseItem(Player player) {
        if (!player.TryGetModPlayer(out SpawnpointPotionPlayer beaconPlayer)) {
            return false;
        }

        beaconPlayer.beaconPos = player.Center.ToTileCoordinates();
        beaconPlayer.beaconPointCd = Cooldown;
        return true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.RecallPotion)
#if POLLUTED_OCEAN
            .AddIngredient<Fishing.Fish.Piraiba>()
#else
            .AddIngredient<global::Aequus.Items.Materials.Fish.IcebergFish>()
#endif
            .AddTile(TileID.Bottles)
            .Register()
            .SortBeforeFirstRecipesOf(ItemID.PotionOfReturn);
    }
}
