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
            return new ArmorCustomTexture(Effect, Pass,
                new Ref<Texture2D>(ModContent.Request<Texture2D>("Aequus/Assets/EnchantGlimmer", AssetRequestMode.ImmediateLoad).Value)).UseOpacity(0.8f);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SilverDye)
                .AddIngredient<CosmicEnergy>()
                .AddTile(TileID.DyeVat)
                .RegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}