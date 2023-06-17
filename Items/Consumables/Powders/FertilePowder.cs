using Aequus.Projectiles.Misc;
using Aequus.Tiles.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Powders {
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
            CreateRecipe(3)
                .AddIngredient<SedimentaryRockItem>(9)
                .AddIngredient(ItemID.Fertilizer, 3)
                .AddTile(TileID.Bottles)
                .Register();
        }
    }
}