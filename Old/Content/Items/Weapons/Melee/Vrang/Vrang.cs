namespace Aequu2.Old.Content.Items.Weapons.Melee.Vrang;

public class Vrang : ModItem {
    public override void SetDefaults() {
        Item.SetWeaponValues(20, 3f, 8);
        Item.width = 24;
        Item.height = 20;
        Item.useAnimation = 40;
        Item.useTime = 40;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.DamageType = DamageClass.Melee;
        Item.value = Item.sellPrice(silver: 75);
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item1;
        Item.channel = true;
        Item.noMelee = true;
        Item.noUseGraphic = true;
        Item.autoReuse = true;
        Item.shootSpeed = 22f;
        Item.shoot = ModContent.ProjectileType<VrangProj>();
    }
}