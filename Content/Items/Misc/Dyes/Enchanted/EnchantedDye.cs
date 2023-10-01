using Aequus.Content.Items.Material.Energy.Cosmic;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Content.Items.Misc.Dyes.Enchanted {
    public class EnchantedDye : DyeItemBase {
        public override int Rarity => ItemRarityID.Orange;

        public override string Pass => "EnchantmentPass";

        public override ArmorShaderData CreateShaderData() {
            return base.CreateShaderData().UseOpacity(0.8f).UseImage(AequusTextures.EnchantedDyeEffect);
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.DyeVat)
                .Register();
        }
    }
}