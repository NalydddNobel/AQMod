using Aequus.Biomes.CrabCrevice.Tiles;
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
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<FertilePowderProj>();
            Item.width = 16;
            Item.height = 24;
            Item.maxStack = 9999;
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
            CreateRecipe(3)
                .AddIngredient<SedimentaryRock>(9)
                .AddIngredient(ItemID.Bone, 9)
                .AddIngredient(ItemID.AshBlock, 9)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}