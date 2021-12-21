using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts
{
    public class DemonSpawnDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        private static Color getColor(float time)
        {
            var c = new Color(255, 100, 100, 10);
            var c2 = new Color(240, 190, 44, 10);
            return Color.Lerp(c, c2, (float)(Math.Sin(time * 10f) + 1f) / 2f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => getColor(Main.GlobalTime + dust.scale * 3f);

        public override bool Update(Dust dust)
        {
            dust.velocity.X *= 0.8f;
            dust.velocity.Y *= 0.98f;
            float velo = dust.velocity.Length();
            dust.scale -= 0.05f - velo / 1000f;
            if (dust.scale <= 0.1f)
                dust.active = false;
            if (!dust.noLight)
                Lighting.AddLight(dust.position, getColor(Main.GlobalTime + dust.scale * 3f).ToVector3() * 0.5f);
            dust.position += dust.velocity;
            return false;
        }
    }
}