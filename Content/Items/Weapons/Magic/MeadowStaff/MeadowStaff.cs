﻿using Aequus.Tiles.Meadow;

namespace Aequus.Content.Items.Weapons.Magic.MeadowStaff;

public class MeadowStaff : ModItem {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        // Duplicate of Sapphire Staff
        Item.mana = 6;
        Item.UseSound = SoundID.Item43;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.damage = 18;
        Item.useAnimation = 34;
        Item.useTime = 34;
        Item.width = 40;
        Item.height = 40;
        Item.shoot = ProjectileID.SapphireBolt;
        Item.shootSpeed = 7.5f;
        Item.knockBack = 4f;
        Item.value = Item.sellPrice(copper: 20);
        Item.DamageType = DamageClass.Magic;
        Item.autoReuse = true;
        Item.noMelee = true;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.GetInstance<MeadowWood>().Item.Type, 14)
            .AddTile(TileID.WorkBenches)
            .Register();
    }
}
