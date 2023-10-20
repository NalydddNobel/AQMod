﻿using Aequus.Common.Items;
using Aequus.Content.Items.Material;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Items.Weapons.Melee.Swords.Slice;

public class Slice : ModItem {
    public override void SetDefaults() {
        Item.LazyCustomSwordDefaults<SliceProj>(30);
        Item.shootSpeed = 15f;
        Item.SetWeaponValues(38, 4.5f, 6);
        Item.width = 20;
        Item.height = 20;
        Item.autoReuse = true;
        Item.rare = ItemCommons.Rarity.SpaceStormLoot;
        Item.value = ItemCommons.Price.SpaceStormLoot;
        Item.scale = 0.9f;
    }

    public override bool? UseItem(Player player) {
        return null;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(120);
    }

    public override bool MeleePrefix() {
        return true;
    }

    public override bool AltFunctionUse(Player player) {
        return false;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.CobaltBar, 14)
            .AddIngredient<FrozenTear>(20)
            .AddTile(TileID.Anvils)
            .Register()
            .Clone().ReplaceItem(ItemID.CobaltBar, ItemID.PalladiumBar).Register();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        return true;
    }
}