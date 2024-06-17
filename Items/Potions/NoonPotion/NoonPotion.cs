﻿using Aequus.Common.Items;
using Aequus.Common.Items.Dedications;

namespace Aequus.Items.Potions.NoonPotion;
public class NoonPotion : ModItem {
    public override void Load() {
        DedicationRegistry.Register(this, new DefaultDedication("Noonison", new Color(200, 80, 50, 255)));
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.DrinkParticleColors[Type] = [new Color(255, 255, 255, 0), new Color(255, 100, 10, 0), new Color(255, 255, 40, 0), new Color(255, 200, 20, 0),];
        Item.ResearchUnlockCount = 20;
    }

    public override void SetDefaults() {
        Item.width = 20;
        Item.height = 26;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useAnimation = 17;
        Item.useTime = 17;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.consumable = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = ItemDefaults.ValueBuffPotion;
        Item.maxStack = Item.CommonMaxStack;
        Item.buffTime = 28800;
        Item.buffType = ModContent.BuffType<NoonBuff>();
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.ShinePotion)
            .AddIngredient(ItemID.NightOwlPotion)
            .AddIngredient(ItemID.FallenStar)
            .AddIngredient(ItemID.SoulofLight)
            .AddTile(TileID.Bottles)
            .TryRegisterBefore(ItemID.InfernoPotion);
    }
}