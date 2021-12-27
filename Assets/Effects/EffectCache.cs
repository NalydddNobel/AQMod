using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AQMod.Assets.Effects
{
    public sealed class EffectCache : ILoadable
    {
        public static Asset<Effect> Trailshader { get; private set; }
        public static Asset<Effect> ParentPixelShader { get; private set; }

        public static MiscShaderData s_Outline { get => GameShaders.Misc["AQMod:Outline"]; set => GameShaders.Misc["AQMod:Outline"] = value; }
        public static MiscShaderData s_Spotlight { get => GameShaders.Misc["AQMod:Spotlight"]; set => GameShaders.Misc["AQMod:Spotlight"] = value; }

        void ILoadable.Load(Mod mod)
        {
            Trailshader = ModContent.Request<Effect>("AQMod/Assets/Effects/Trails/Trailshader", AssetRequestMode.AsyncLoad);
            ParentPixelShader = ModContent.Request<Effect>("AQMod/Assets/Effects/Dyes/ParentDyeShader", AssetRequestMode.ImmediateLoad);

            s_Outline = new MiscShaderData(new Ref<Effect>(ParentPixelShader.Value), "OutlineColorPass");
            s_Spotlight = new MiscShaderData(new Ref<Effect>(ParentPixelShader.Value), "SpotlightPass");
        }

        void ILoadable.Unload()
        {
            ParentPixelShader = null;
            Trailshader = null;
        }
    }
}