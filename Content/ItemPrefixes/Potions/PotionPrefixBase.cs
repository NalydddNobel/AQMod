using Aequus.Common.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Content.ItemPrefixes.Potions {
    public abstract class PotionPrefixBase : AequusPrefix {
        private string shaderKey;
        public string ShaderKey => shaderKey;

        public abstract string GlintTexture { get; }
        public bool HasGlint { get; private set; }

        public override void Load() {
            string glintTexture = GlintTexture;
            if (string.IsNullOrEmpty(glintTexture)) {
                return;
            }

            HasGlint = true;
            shaderKey = $"Aequus:{Name}";
            GameShaders.Misc[shaderKey] = new MiscShaderData(
                AequusShaders.GlintMiscShader,
                "EnchantmentPass")
                .UseOpacity(0.8f)
                .UseImage1(ModContent.Request<Texture2D>(glintTexture));
        }
    }
}