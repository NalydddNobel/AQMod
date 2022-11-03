using Aequus.Graphics.ShaderData;
using Microsoft.Xna.Framework;
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
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                return new Color(v).HueAdd(0.01f * Main.GlobalTimeWrappedHourly).ToVector3();
            }).UseOpacity(1f);
        }
    }
}