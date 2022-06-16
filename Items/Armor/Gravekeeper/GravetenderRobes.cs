using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Gravekeeper
{
    public class GravetenderRobes : ModItem
    {
        public override string Texture => Aequus.VanillaTexture + "Item_" + ItemID.LeadChainmail;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LeadChainmail);
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