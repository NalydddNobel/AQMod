using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Items.Vanities.Dyes
{
    public class EnchantedDye : DyeItem
    {
        public override string Pass => "ImageScrollPass";
        public override ArmorShaderData CreateShaderData => new CustomTextureArmorShaderData(Effect, Pass,
            new Ref<Texture2D>(ModContent.GetTexture("AQMod/Assets/Textures/EnchantGlimmer")));
    }
}