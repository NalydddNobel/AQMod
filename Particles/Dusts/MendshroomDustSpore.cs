using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Aequus.Particles.Dusts {
    public class MendshroomDustSpore : MonoDust
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
                dust.alpha -= 5;
                if (dust.alpha < 0)
                {
                    dust.alpha = 0;
                }
            }
            else
            {
                dust.scale -= 0.01f - Math.Min(dust.scale * 0.009f, 0.005f);
                if (dust.scale < 0.01f)
                {
                    dust.active = false;
                }
            }

            dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
            if (dust.velocity.Length() > 0.1f)
            {
                dust.velocity *= 0.99f;
            }
            dust.rotation += 0.005f;

            dust.position += dust.velocity;
            return false;
        }
    }
}