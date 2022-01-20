using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AQMod.Items.Dyes
{
    public class DiscoDye : DyeItem
    {
        public override string Pass => "RainbowPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                float multiplier = (v.X + v.Y + v.Z) / 3f;
                float rainbowTime = Main.GlobalTime * 6 + (v.X + v.Y + v.Z);
                return new Vector3((float)Math.Sin(rainbowTime), (float)Math.Sin(rainbowTime + MathHelper.TwoPi / 3f), (float)Math.Sin(rainbowTime + MathHelper.TwoPi / 3f * 2f)) * multiplier;
            }).UseOpacity(1f);
        }
    }
}