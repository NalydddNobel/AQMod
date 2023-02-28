using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Items.Misc.Dyes
{
    public class HueshiftDye : DyeItemBase
    {
        public override Ref<Effect> Effect => FromPath("Dyes/HueshiftDyeShader");
        public override string Pass => "HueShiftPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderData(Effect, Pass).UseOpacity(1f);
        }
    }
}