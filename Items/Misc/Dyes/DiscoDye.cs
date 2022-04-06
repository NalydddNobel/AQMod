using Aequus.Effects.Armor;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes
{
    public class DiscoDye : DyeItemBase
    {
        public override int Rarity => ItemRarityID.Orange;

        public override string Pass => "RainbowPass";

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                float multiplier = (v.X + v.Y + v.Z) / 3f;
                float rainbowTime = Main.GlobalTimeWrappedHourly * 6 + (v.X + v.Y + v.Z);
                return new Vector3((float)Math.Sin(rainbowTime), (float)Math.Sin(rainbowTime + MathHelper.TwoPi / 3f), (float)Math.Sin(rainbowTime + MathHelper.TwoPi / 3f * 2f)) * multiplier;
            }).UseOpacity(1f);
        }
    }
}