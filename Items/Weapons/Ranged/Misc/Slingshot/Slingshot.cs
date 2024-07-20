using Aequus.Projectiles.Ranged;

namespace Aequus.Items.Weapons.Ranged.Misc.Slingshot;
public class Slingshot : ModItem {
    public override void SetDefaults() {
        Item.damage = 25;
        Item.DamageType = DamageClass.Ranged;
        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.width = 32;
        Item.height = 24;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ModContent.ProjectileType<SlingshotProj>();
        Item.noUseGraphic = true;
        Item.shootSpeed = 7.5f;
        Item.UseSound = AequusSounds.stretch with { Volume = 0.2f };
        Item.value = Item.sellPrice(gold: 2);
        Item.knockBack = 1f;
        Item.useAmmo = SlingshotAmmos.BirdAmmo;
        Item.channel = true;
    }
}