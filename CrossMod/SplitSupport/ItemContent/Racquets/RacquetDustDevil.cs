using Aequus.Common.Items;
using Aequus.Items.Materials.Energies;
using Aequus.Items.Materials.GaleStreams;
using Terraria.ID;

namespace Aequus.CrossMod.SplitSupport.ItemContent.Racquets;
public class RacquetDustDevil : RacquetBase {
    public override int BallCount => 2;

    public override void SetDefaults() {
        base.SetDefaults();

        Item.SetWeaponValues(30, 2f, 0);
        Item.width = 32;
        Item.height = 32;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.value = ItemDefaults.ValueDustDevil;
        Item.rare = ItemDefaults.RarityDustDevil;
        Item.autoReuse = true;
        //Item.shoot = ModContent.ProjectileType<BaseballDustDevil>();
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient<Fluorescence>(7)
            .AddIngredient<FrozenTear>(7)
            .AddIngredient<AtmosphericEnergy>()
            .AddTile(TileID.Anvils);
    }
}