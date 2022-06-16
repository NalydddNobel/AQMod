using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Gravetender
{
    public class GravetenderRobes : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 3;
            Item.width = 20;
            Item.height = 20;
            Item.bodySlot = 1;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost -= 0.15f;
            player.Aequus().ghostSlotsMax++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 12 * 7)
                .AddIngredient(ItemID.RottenChunk, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.GravediggerShovel));
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 12 * 7)
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.GravediggerShovel));
        }
    }
}