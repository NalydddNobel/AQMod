using Aequus.Common.Effects.ShaderData;
using Aequus.Items.Materials.Energies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.Dyes
{
    public class GustDye : DyeItemBase
    {
        public override Ref<Effect> Effect => FromPath("Dyes/GustDyeShader");
        public override string Pass => "ModdersToolkitShaderPass";
        public override int Rarity => ItemDefaults.RarityGaleStreams;

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorCustomTexture(Effect, Pass, ModContent.Request<Texture2D>($"{Aequus.VanillaTexture}Misc/Perlin")).UseOpacity(0.8f).UseColor(new Vector3(1f, 1f, 0.33f));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<AtmosphericEnergy>()
                .AddTile(TileID.DyeVat)
                .TryRegisterAfter(ItemID.IntenseRainbowDye);
        }
    }
}