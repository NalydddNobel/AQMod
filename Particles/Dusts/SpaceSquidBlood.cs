using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles.Dusts
{
    public class SpaceSquidBlood : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 100;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(Math.Max(lightColor.R, (byte)60), Math.Max(lightColor.G, (byte)60), Math.Max(lightColor.B, (byte)60), 255 - dust.alpha);
        }
    }
}