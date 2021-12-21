using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts.NobleMushrooms
{
    public class ArgonDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.alpha = 255;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(255, 255, 255, 255 - dust.alpha);
    }
}