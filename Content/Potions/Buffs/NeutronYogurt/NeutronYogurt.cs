using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Aequus.Content.Potions.Buffs.NeutronYogurt;

public class NeutronYogurt : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 20;
        Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));
        ItemSets.DrinkParticleColors[Type] = new[] { Color.HotPink, Color.Yellow };
    }

    public override void SetDefaults() {
        Item.width = 10;
        Item.height = 10;
        Item.useStyle = ItemUseStyleID.DrinkLiquid;
        Item.useAnimation = 15;
        Item.useTime = 15;
        Item.useTurn = true;
        Item.UseSound = SoundID.Item3;
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.sellPrice(silver: 2);
        Item.buffType = ModContent.BuffType<NeutronYogurtBuff>();
        Item.buffTime = 28800;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.FallenStar, 5)
            .AddTile(TileID.CookingPots)
            .Register();
    }
}