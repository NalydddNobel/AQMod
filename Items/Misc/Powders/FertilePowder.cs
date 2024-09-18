﻿using Aequus.Projectiles.Misc;

namespace Aequus.Items.Misc.Powders;
public class FertilePowder : ModItem {
    public override void SetDefaults() {
        Item.damage = 0;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.shootSpeed = 5f;
        Item.shoot = ModContent.ProjectileType<FertilePowderProj>();
        Item.width = 16;
        Item.height = 24;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.UseSound = SoundID.Item1;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.noMelee = true;
        Item.value = Item.sellPrice(silver: 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void AddRecipes() {
        int boxItem = ItemID.Sandstone;
#if !CRAB_CREVICE_DISABLE
        boxItem = ModContent.ItemType<Tiles.CrabCrevice.SedimentaryRockItem>();
#endif

        CreateRecipe(3)
            .AddIngredient(boxItem, 9)
            .AddIngredient(ItemID.Fertilizer, 3)
            .AddTile(TileID.Bottles)
            .Register();
    }
}