using Aequus.Common.Items;
using Aequus.Items.Materials.Glimmer;
using Terraria.ID;

namespace Aequus.CrossMod.SplitSupport.ItemContent.Racquets; 
public class RacquetStarite : RacquetBase {
    public override int BallCount => 5;

    public override void SetDefaults() {
        base.SetDefaults();

        Item.SetWeaponValues(8, 4f, 0);
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.value = ItemDefaults.ValueGlimmer;
        Item.rare = ItemDefaults.RarityGlimmer;
        Item.autoReuse = true;
        //Item.shoot = ModContent.ProjectileType<BaseballStarite>();
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<StariteMaterial>(15)
            .AddIngredient(ItemID.FallenStar, 10)
            .AddTile(TileID.Anvils);
    }
}