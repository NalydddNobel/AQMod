namespace Aequus.Old.Content.Items.Weapons.Ranged.CrusadersCrossbow;

public class CrusadersCrossbow : ModItem {
    public override void SetDefaults() {
        Item.damage = 22;
        Item.DamageType = DamageClass.Ranged;
        Item.useTime = 32;
        Item.useAnimation = 32;
        Item.width = 30;
        Item.height = 30;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.shootSpeed = 14f;
        Item.useAmmo = AmmoID.Arrow;
        Item.UseSound = SoundID.Item5;
        Item.value = Item.sellPrice(gold: 1);
        Item.noMelee = true;
        Item.knockBack = 0.5f;
        Item.autoReuse = true;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-4f, 0f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
        if (type == ProjectileID.WoodenArrowFriendly) {
            type = ModContent.ProjectileType<CrusadersCrossbowProj>();
        }
    }
}