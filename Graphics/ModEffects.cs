using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public sealed class ModEffects : ILoadable
    {
        public static StaticMiscShaderInfo<ShaderData> VerticalGradient { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            VerticalGradient = new StaticMiscShaderInfo<ShaderData>("MiscEffects", "Aequus:VerticalGradient", "VerticalGradientPass", true);
        }

        void ILoadable.Unload()
        {
            VerticalGradient = null;
        }
    }
}