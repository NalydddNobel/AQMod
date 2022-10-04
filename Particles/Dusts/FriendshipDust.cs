using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles.Dusts
{
    public class FriendshipDust : ModDust
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
            dust.velocity.X *= 0.95f;
            dust.velocity.Y *= dust.velocity.Y < 0f ? 0.97f : 0.95f;
            dust.velocity = dust.velocity.RotatedBy(0.02f);
            float speed = dust.velocity.Length();
            if (dust.fadeIn > 0f)
            {
                dust.scale += 0.05f;
                if (dust.scale > dust.fadeIn)
                {
                    dust.fadeIn = 0f;
                }
                if (dust.alpha > 0)
                {
                    dust.alpha -= 10;
                    if (dust.alpha < 0)
                        dust.alpha = 0;
                }
            }
            else
            {
                dust.scale -= 0.05f - speed / 1000f;
            }
            dust.rotation = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f) * dust.scale * 0.2f;
            if (dust.scale <= 0.1f)
                dust.active = false;

            if (!dust.noLight)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);
            dust.position += dust.velocity;
            return false;
        }
    }
}