using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public sealed class ModEffects
    {
        private Asset<Effect> necromancyOutline;
        public Asset<Effect> NecromancyOutline { get => necromancyOutline; }
        public static MiscShaderData NecromancyOutlineShader { get => GameShaders.Misc[NecromancyOutlineKey]; set => GameShaders.Misc[NecromancyOutlineKey] = value; }
        public const string NecromancyOutlineKey = "Aequus:NecromancyOutline";

        private Asset<Effect> miscEffect;
        public Asset<Effect> MiscEffect { get => miscEffect; }
        public static MiscShaderData MiscShader { get => GameShaders.Misc[MiscShaderKey]; set => GameShaders.Misc[MiscShaderKey] = value; }
        public const string MiscShaderKey = "Aequus:Misc";

        public ModEffects()
        {
            LoadShaders();
        }
        private void LoadShaders()
        {
            LoadMiscShader(ref necromancyOutline, "NecromancyOutline", NecromancyOutlineKey, "NecromancyOutlinePass");
            LoadMiscShader(ref miscEffect, "MiscEffects", MiscShaderKey, "VerticalGradientPass");
        }
        public void LoadMiscShader(ref Asset<Effect> asset, string requestPath, string gameShaderKey, string pass)
        {
            asset = ModContent.Request<Effect>("Aequus/Assets/Effects/" + requestPath, AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[gameShaderKey] = new MiscShaderData(new Ref<Effect>(asset.Value), pass);
        }
    }
}