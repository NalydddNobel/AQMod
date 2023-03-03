using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes
{
    public class TidalDye : DyeItemBase
    {
        public override Ref<Effect> Effect => FromAssetFolder("Dyes/TidalDyeShader");
        public override string Pass => "TidalDyePass";

        public override ArmorShaderData CreateShaderData()
        {
            return base.CreateShaderData().UseColor(new Vector3(0f, 0.3f, 1f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}