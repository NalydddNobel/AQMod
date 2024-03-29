﻿using Aequus.Items.Materials.Fish;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.FrostPotion; 

public class FrostPotion : ModItem {
    public const float DamageMultiplier = 0.7f;

    public override void SetStaticDefaults() {
        ItemID.Sets.DrinkParticleColors[Type] = new Color[] { new Color(12, 237, 255, 0), new Color(53, 202, 255, 0), new Color(0, 156, 255, 0), new Color(0, 74, 205, 0), };
        Item.ResearchUnlockCount = 20;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.WarmthPotion);
        Item.buffType = ModContent.BuffType<FrostBuff>();
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.BottledWater)
            .AddIngredient<HeatFish>()
            .AddIngredient(ItemID.Fireblossom)
            .AddTile(TileID.Bottles)
            .TryRegisterBefore(ItemID.WarmthPotion);
    }
}