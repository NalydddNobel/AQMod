using AQMod.Assets.Effects.Dyes;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Dyes
{
    public class EnchantedDye : DyeItem
    {
        public override string Pass => "EnchantmentPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new CustomTextureArmorShaderData(Effect, Pass,
                new Ref<Texture2D>(ModContent.Request<Texture2D>("AQMod/Assets/Samplers/EnchantGlimmer", AssetRequestMode.ImmediateLoad).Value)).UseOpacity(0.8f);
        }
    }
}