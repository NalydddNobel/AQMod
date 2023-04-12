using Aequus.Common.Effects.ShaderData;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes.Ancient
{
    public class AncientTidalDye : DyeItemBase
    {
        public override string Pass => "AquaticShaderPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorCustomTexture(Effect, Pass, ModContent.Request<Texture2D>("Aequus/Assets/Effects/Textures/TidalDye")).UseOpacity(1f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<AquaticEnergy>()
                .AddTile(TileID.DyeVat)
                .AddCondition(Condition.InGraveyard)
                .TryRegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}