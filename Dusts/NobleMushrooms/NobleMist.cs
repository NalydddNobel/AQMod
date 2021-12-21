using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts.NobleMushrooms
{
    public abstract class NobleMist : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.alpha = 128;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(lightColor.R * lightColor.R, lightColor.G * lightColor.G, lightColor.B * lightColor.B, 255 - dust.alpha);

        public sealed override bool Update(Dust dust)
        {
            dust.velocity.X = ((float)Math.Sin(dust.position.Y / 32f + Main.GlobalTime + dust.dustIndex * 0.00125f) + 1f) * 1.25f + 0.25f;
            if (dust.position.Y < Main.worldSurface * 16f)
                dust.velocity.X *= Main.windSpeed;
            dust.velocity.Y = -(((float)Math.Sin(dust.position.X / 32f + Main.GlobalTime + dust.dustIndex * 0.001f) + 1f) * 1.25f + 0.25f);
            if (dust.scale <= 0.1f)
                dust.active = false;
            if (dust.customData == null || dust.customData.GetType() != typeof(int))
                dust.customData = 0;
            dust.customData = (int)dust.customData + 1;

            dust.scale = (float)Math.Sin((dust.position.X + dust.position.Y) * 0.0125f) * 0.125f + 0.75f - (int)dust.customData * 0.0025f;

            if (dust.scale <= 0.1f)
                dust.active = false;

            dust.position += dust.velocity;
            return false;
        }
    }
}