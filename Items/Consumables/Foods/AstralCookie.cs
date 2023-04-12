using Aequus.Buffs;
using Aequus.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables.Foods
{
    public class AstralCookie : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
            this.StaticDefaultsToFood(new Color(150, 100, 100, 255), new Color(150, 100, 100, 255), new Color(150, 100, 100, 255), new Color(55, 35, 35, 255), new Color(120, 10, 150, 255));
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(20, 20, ModContent.BuffType<AstralCookieBuff>(), 36000);
            Item.rare = ItemDefaults.RarityGlimmer;
            Item.value = Item.sellPrice(silver: 20);
        }

        public override void AddRecipes() {

            CreateRecipe()
                .AddIngredient<StariteMaterial>()
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}