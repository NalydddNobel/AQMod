using Aequus.Graphics.ArmorShaders;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Aequus.Items.Misc.Dyes
{
    public class BreakdownDye : DyeItemBase
    {
        public override string Pass => "ColorDistortPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                var color = v;
                float time = Main.GlobalTimeWrappedHourly * 20;
                float rLERP = (float)(Math.Sin(color.X * 10 + time) + 1f) / 2f;
                color.X = MathHelper.Lerp(color.X, rLERP, 0.5f);
                float gLERP = (float)Math.Sin(color.Y * 10 + time) / 2f;
                color.Y = MathHelper.Lerp(color.Y, gLERP, 0.5f);
                float bLERP = (float)Math.Sin(color.Z * 10 + time + color.X) / 2f;
                color.Z = MathHelper.Lerp(color.Z, bLERP, 0.5f);
                return color;
            }).UseOpacity(1f);
        }
    }
}