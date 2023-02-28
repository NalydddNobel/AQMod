using Aequus.Items.Materials.Energies;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes
{
    public class DiscoDye : DyeItemBase
    {
        public override int Rarity => ItemRarityID.Orange;

        public override string Pass => "RainbowPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderData(Effect, Pass).UseOpacity(1f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<UltimateEnergy>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}