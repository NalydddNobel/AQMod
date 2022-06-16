using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Gravekeeper
{
    public class GravetenderHood : ModItem
    {
        public override string Texture => Aequus.VanillaTexture + "Item_" + ItemID.LeadHelmet;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.LeadHelmet);
            Item.rare = ItemRarityID.Blue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GravetenderRobes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = AequusText.GetText("ArmorSetBonus.Gravetender");
            player.Aequus().setGravetender = Item;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 8 * 7)
                .AddIngredient(ItemID.RottenChunk, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.GravediggerShovel));
            CreateRecipe()
                .AddIngredient(ItemID.Cobweb, 8 * 7)
                .AddIngredient(ItemID.Vertebrae, 5)
                .AddTile(TileID.Loom)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register((r) => r.SortBeforeFirstRecipesOf(ItemID.GravediggerShovel));
        }
    }
}