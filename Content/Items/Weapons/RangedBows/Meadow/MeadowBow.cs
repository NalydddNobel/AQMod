using Aequus.Common;

namespace Aequus.Content.Items.Weapons.RangedBows.Meadow;

[WorkInProgress]
public class MeadowBow : ModItem {
    public override void SetDefaults() {
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 23;
        Item.useTime = 23;
        Item.width = 12;
        Item.height = 28;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.useAmmo = AmmoID.Arrow;
        Item.UseSound = SoundID.Item5;
        Item.damage = 9;
        Item.shootSpeed = 10f;
        Item.noMelee = true;
        Item.value = Item.sellPrice(copper: 20);
        Item.DamageType = DamageClass.Ranged;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(4f, 0f);
    }

    public override void AddRecipes() {
#if DEBUG
        CreateRecipe()
            .AddIngredient(ModContent.GetInstance<Biomes.Meadows.Tiles.MeadowWood>().Item.Type, 10)
            .AddTile(TileID.WorkBenches)
            .Register();
#endif
    }
}
