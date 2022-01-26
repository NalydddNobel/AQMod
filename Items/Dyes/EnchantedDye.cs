using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Dyes
{
    public class EnchantedDye : DyeItem
    {
        public override int Rarity => ItemRarityID.Orange;

        public override string Pass => "EnchantmentPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new CustomTextureArmorShaderData(Effect, Pass,
                new Ref<Texture2D>(ModContent.GetTexture("AQMod/Effects/EnchantGlimmer"))).UseOpacity(0.8f);
        }
    }
}