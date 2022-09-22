using Aequus.Items.Placeable.CrabCrevice;
using Aequus.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class FertilePowder : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<FertilePowderProj>();
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 99;
            Item.consumable = true;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.noMelee = true;
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SedimentaryRock>(5)
                .AddIngredient(ItemID.PurificationPowder)
                .Register();
        }
    }
}