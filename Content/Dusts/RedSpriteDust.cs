using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Content.Dusts
{
    public class RedSpriteDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 200;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            if (lightColor.R < 60)
            {
                lightColor.R = 60;
            }
            if (lightColor.G < 60)
            {
                lightColor.G = 60;
            }
            if (lightColor.B < 60)
            {
                lightColor.B = 60;
            }
            return new Color(lightColor.R, lightColor.G, lightColor.B, 255 - dust.alpha);
        }
    }
}