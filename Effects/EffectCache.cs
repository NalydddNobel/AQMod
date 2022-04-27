using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Effects
{
    public sealed class EffectCache
    {
        private Asset<Effect> miscEffect;
        public Asset<Effect> MiscEffect { get => miscEffect; }
        public static MiscShaderData MiscShader { get => GameShaders.Misc["Aequus:Misc"]; set => GameShaders.Misc["Aequus:Misc"] = value; }
        public const string MiscShaderKey = "Aequus:Misc";

        public EffectCache()
        {
            LoadShaders();
        }
        private void LoadShaders()
        {
            LoadMiscShader(ref miscEffect, "MiscEffects", MiscShaderKey, "VerticalGradientPass");
        }
        private void LoadMiscShader(ref Asset<Effect> asset, string requestPath, string gameShaderKey, string pass)
        {
            asset = ModContent.Request<Effect>("Aequus/Assets/Effects/" + requestPath, AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[gameShaderKey] = new MiscShaderData(new Ref<Effect>(asset.Value), pass);
        }
    }
}