using AQMod.Effects.Dyes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts
{
    public class SparklerDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color * (1f - dust.alpha / 255f);

        public override bool Update(Dust dust)
        {
            dust.velocity = dust.velocity.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f));
            float velo = dust.velocity.Length();
            dust.rotation += velo * 0.0314f;
            dust.scale -= 0.05f - velo / 1000f;
            dust.alpha += 5;
            if (dust.scale <= 0.1f || dust.alpha >= 255)
                dust.active = false;
            if (!dust.noLight)
            {
                Vector3 lightColor;
                if (dust.shader != null)
                    lightColor = DyeHelper.ModifyLight(dust.shader, dust.color.ToVector3() * 0.5f);
                else
                {
                    lightColor = dust.color.ToVector3() * 0.5f;
                }
                Lighting.AddLight(dust.position, lightColor);
            }
            dust.position += dust.velocity;
            return false;
        }
    }
}