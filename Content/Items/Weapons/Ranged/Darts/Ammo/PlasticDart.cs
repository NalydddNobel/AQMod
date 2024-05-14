using Aequus.Content.Items.Materials;

namespace Aequus.Content.Items.Weapons.Ranged.Darts.Ammo;

public class PlasticDart : ModItem {
    public override void SetDefaults() {
        Item.damage = 12;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 8;
        Item.height = 8;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.knockBack = 1.5f;
        Item.value = Item.sellPrice(copper: 1);
        Item.rare = ItemRarityID.White;
        Item.shoot = ModContent.ProjectileType<PlasticDartProjectile>();
        Item.shootSpeed = 16f;
        Item.ammo = AmmoID.Dart;
    }

    public override void AddRecipes() {
        CreateRecipe(150)
            .AddIngredient<CompressedTrash>()
            .AddTile(TileID.Anvils)
            .Register();
    }
}