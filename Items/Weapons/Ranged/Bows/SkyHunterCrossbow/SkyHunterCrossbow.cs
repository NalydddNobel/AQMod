using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Bows.SkyHunterCrossbow;

public class SkyHunterCrossbow : ModItem {
    public override void SetDefaults() {
        Item.SetWeaponValues(20, 3f);
        Item.DamageType = DamageClass.Ranged;
        Item.useAmmo = AmmoID.Arrow;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ProjectileID.WoodenArrowFriendly;
        Item.UseSound = SoundID.Item5;
        Item.shootSpeed = 6.7f;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(-10f, 0f);
    }
}