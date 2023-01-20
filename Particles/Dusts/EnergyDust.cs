using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles.Dusts
{
    public class EnergyDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.alpha = 0;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(dust.color.R - dust.alpha, dust.color.G - dust.alpha, dust.color.B - dust.alpha, dust.color.A - dust.alpha);

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.92f;
            float velo = dust.velocity.Length();
            if (!dust.noGravity)
            {
                dust.rotation += velo * 0.0314f;
                dust.velocity.Y *= 0.96f;
            }
            dust.scale -= 0.01f - velo / 1000f + Math.Max(1f - dust.scale, 0f) * 0.1f;
            dust.alpha += (int)velo * 4;
            dust.rotation += 0.02f + Math.Max(1f - dust.scale, 0f);
            if (dust.alpha >= 255)
                dust.active = false;
            if (dust.scale <= 0.1f)
                dust.active = false;
            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);
            dust.position += dust.velocity;
            return false;
        }
    }
}