using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace AQMod.Assets
{
    public static class EffectCache
    {
        public static Effect ParentPixelShader { get; internal set; }
        public static Effect ParentScreenShader { get; internal set; }
        public static Effect GoreNestPortal { get; private set; }
        public static Effect Trailshader { get; private set; }

        public static MiscShaderData s_Outline { get => GameShaders.Misc["AQMod:Outline"]; set => GameShaders.Misc["AQMod:Outline"] = value; }
        public static MiscShaderData s_OutlineColor { get => GameShaders.Misc["AQMod:OutlineColor"]; set => GameShaders.Misc["AQMod:OutlineColor"] = value; }
        public static MiscShaderData s_GoreNestPortal { get => GameShaders.Misc["AQMod:GoreNestPortal"]; set => GameShaders.Misc["AQMod:GoreNestPortal"] = value; }
        public static MiscShaderData s_Spotlight { get => GameShaders.Misc["AQMod:Spotlight"]; set => GameShaders.Misc["AQMod:Spotlight"] = value; }
        public static MiscShaderData s_FadeYProgressAlpha { get => GameShaders.Misc["AQMod:FadeYProgressAlpha"]; set => GameShaders.Misc["AQMod:FadeYProgressAlpha"] = value; }
        public static MiscShaderData s_SpikeFade { get => GameShaders.Misc["AQMod:SpikeFade"]; set => GameShaders.Misc["AQMod:SpikeFade"] = value; }
        public static MiscShaderData s_Enchant { get => GameShaders.Misc["AQMod:Enchant"]; set => GameShaders.Misc["AQMod:Enchant"] = value; }

        public static Filter f_Vignette { get => Filters.Scene["AQMod:Vignette"]; set => Filters.Scene["AQMod:Vignette"] = value; }

        internal static void Load(AQMod aQMod)
        {
            AQMod.Debug.DebugLogger? logger = null;
            if (AQMod.Debug.LogEffectLoading)
                logger = AQMod.Debug.GetDebugLogger();
            ParentPixelShader = logGetEffect("Dyes/ParentDyeShader", aQMod, logger);
            ParentScreenShader = logGetEffect("ParentScreenShader", aQMod, logger);
            GoreNestPortal = logGetEffect("GoreNest/GoreNestPortal", aQMod, logger);
            Trailshader = logGetEffect("Trails/Trailshader", aQMod, logger);

            s_Outline = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "OutlinePass");
            s_OutlineColor = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "OutlineColorPass");
            s_GoreNestPortal = new MiscShaderData(new Ref<Effect>(GoreNestPortal), "DemonicPortalPass");
            s_Spotlight = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "SpotlightPass");
            s_FadeYProgressAlpha = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "FadeYProgressAlphaPass");
            s_SpikeFade = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "SpikeFadePass");
            s_Enchant = new MiscShaderData(new Ref<Effect>(ParentPixelShader), "ImageScrollPass");

            f_Vignette = new Filter(new ScreenShaderData(new Ref<Effect>(ParentScreenShader), "VignettePass"), EffectPriority.Medium);
        }

        private static Effect logGetEffect(string name, AQMod aQMod, AQMod.Debug.DebugLogger? logger)
        {
            if (logger != null)
                logger.Value.Log("Loading effect: Effects/" + name);
            var effect = aQMod.GetEffect("Effects/" + name);
            return effect;
        }
    }
}