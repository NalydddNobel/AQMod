using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Assets
{
    public static class EffectCache
    {
        public static Effect ParentPixelShader { get; internal set; }
        public static Effect GoreNestPortal { get; private set; }
        public static Effect Trailshader { get; private set; }

        public static MiscShaderData s_Outline { get => GameShaders.Misc["AQMod:Outline"]; set => GameShaders.Misc["AQMod:Outline"] = value; }
        public static MiscShaderData s_OutlineColor { get => GameShaders.Misc["AQMod:OutlineColor"]; set => GameShaders.Misc["AQMod:OutlineColor"] = value; }
        public static MiscShaderData s_GoreNestPortal { get => GameShaders.Misc["AQMod:GoreNestPortal"]; set => GameShaders.Misc["AQMod:GoreNestPortal"] = value; }
        public static MiscShaderData s_Spotlight { get => GameShaders.Misc["AQMod:Spotlight"]; set => GameShaders.Misc["AQMod:Spotlight"] = value; }
        public static MiscShaderData s_FadeYProgressAlpha { get => GameShaders.Misc["AQMod:FadeYProgressAlpha"]; set => GameShaders.Misc["AQMod:FadeYProgressAlpha"] = value; }
        public static MiscShaderData s_SpikeFade { get => GameShaders.Misc["AQMod:SpikeFade"]; set => GameShaders.Misc["AQMod:SpikeFade"] = value; }
        public static MiscShaderData s_Enchant { get => GameShaders.Misc["AQMod:Enchant"]; set => GameShaders.Misc["AQMod:Enchant"] = value; }

        internal static void Load(AQMod aQMod)
        {
            ParentPixelShader = logGetEffect("Dyes/ParentDyeShader", aQMod);
            GoreNestPortal = logGetEffect("GoreNest/GoreNestPortal", aQMod);
            Trailshader = logGetEffect("Trails/Trailshader", aQMod);

            s_Outline = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "OutlinePass");
            s_OutlineColor = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "OutlineColorPass");
            s_GoreNestPortal = new MiscShaderData(new Ref<Effect>(GoreNestPortal), "DemonicPortalPass");
            s_Spotlight = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "SpotlightPass");
            s_FadeYProgressAlpha = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "FadeYProgressAlphaPass");
            s_SpikeFade = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "SpikeFadePass");
            s_Enchant = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "ImageScrollPass");
        }

        private static Effect logGetEffect(string path, AQMod aQMod)
        {
            aQMod.Logger.Info("Loading effect: Effects/" + path);
            var effect = aQMod.GetEffect("Effects/" + path);
            if (effect == null)
                aQMod.Logger.Error("Failed to load effect");
            return effect;
        }
    }
}