using Terraria;
using Terraria.ID;

namespace Aequus.Items.Accessories
{
    public sealed class GlowBand : GlowCore
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.defense = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlowCore>()
                .AddIngredient(ItemID.Shackle)
                .AddTile(TileID.WorkBenches)
                .Register();
            ColorRecipes<GlowBand>();
        }
    }
}