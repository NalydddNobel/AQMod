using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Particles.Dusts
{
    public class ReliefshroomDustSpore : MonoDust
    {
        public override float VelocityMultiplier => 0.95f;
        public override float ScaleSubtraction => 0.03f;

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            lightColor = lightColor.MaxRGBA(200);
            return new Color(lightColor.R - dust.alpha, lightColor.G - dust.alpha, lightColor.B - dust.alpha, lightColor.A / 4 - dust.alpha);
        }

        public override bool Update(Dust dust)
        {
            if (dust.alpha > 0)
            {
                dust.alpha -= 10;
                if (dust.alpha < 0)
                {
                    dust.alpha = 0;
                }
            }
            else
            {
                dust.scale -= 0.02f - Math.Min(dust.scale * 0.009f, 0.007f);
                if (dust.scale < 0.01f)
                {
                    dust.active = false;
                }
            }

            float range = 0.15f;
            if (dust.customData is Player player)
            {
                dust.position += player.velocity * Math.Clamp(dust.scale, 0.1f, 1f);
                range *= 1f - Math.Clamp(dust.scale, 0f, 1f);
            }
            dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-range, range));
            float l = dust.velocity.Length();
            if (l > 0.3f)
            {
                dust.velocity *= 0.98f;
            }
            dust.rotation += 0.01f + l * 0.05f;

            dust.position += dust.velocity;
            return false;
        }
    }
}