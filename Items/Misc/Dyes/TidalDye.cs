using Aequus.Graphics.ShaderData;
using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes
{
    public class TidalDye : DyeItemBase
    {
        public override string Pass => "AquaticShaderPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorCustomTexture(Effect, Pass, ModContent.Request<Texture2D>("Aequus/Assets/Effects/Textures/TidalDye")).UseOpacity(1f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}