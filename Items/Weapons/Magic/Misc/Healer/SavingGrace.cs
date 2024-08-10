﻿using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Aequus.Items.Weapons.Melee.Swords.Nettlebane;
using Aequus.Projectiles.Magic;

namespace Aequus.Items.Weapons.Magic.Misc.Healer;
public class SavingGrace : ModItem {
    public override void SetStaticDefaults() {
        Item.staff[Type] = true;
    }

    public override void SetDefaults() {
        Item.damage = 94;
        Item.DamageType = DamageClass.Magic;
        Item.useTime = 50;
        Item.useAnimation = 50;
        Item.width = 20;
        Item.height = 20;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemDefaults.RarityEarlyHardmode;
        Item.shoot = ModContent.ProjectileType<SavingGraceProj>();
        Item.shootSpeed = 30f;
        Item.mana = 80;
        Item.autoReuse = true;
        Item.UseSound = AequusSounds.savingGraceCast;
        Item.value = ItemDefaults.ValueEarlyHardmode;
    }

    public override bool CanUseItem(Player player) {
        return !player.manaSick;
    }

    public override Vector2? HoldoutOffset() {
        return new Vector2(2f, 2f);
    }

    public override void AddRecipes() {
        AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<Nettlebane>());
    }
}