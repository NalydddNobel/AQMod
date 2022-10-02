using Aequus.Graphics.ShaderData;
using Aequus.Items.Misc.Energies;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes
{
    public class EnchantedDye : DyeItemBase
    {
        public override int Rarity => ItemRarityID.Orange;

        public override string Pass => "EnchantmentPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorCustomTexture(Effect, Pass, ModContent.Request<Texture2D>("Aequus/Assets/EnchantGlimmer")).UseOpacity(0.8f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bottle)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}