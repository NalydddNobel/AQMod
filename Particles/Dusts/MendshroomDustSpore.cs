using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Particles.Dusts
{
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
            return new Color(lightColor.R - dust.alpha, lightColor.G - dust.alpha, lightColor.B - dust.alpha, lightColor.A - dust.alpha);
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
                dust.scale -= 0.01f;
                if (dust.scale < 0.01f)
                {
                    dust.active = false;
                }
            }

            dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
            dust.rotation += dust.velocity.Length() * 0.4f;

            dust.position += dust.velocity;


            return true;
        }
    }
}