using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts
{
    public class MonoDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.9f;
            float velo = dust.velocity.Length();
            dust.rotation += velo * 0.0314f;
            dust.scale -= 0.05f - velo / 1000f;
            if (dust.scale <= 0.1f)
                dust.active = false;
            if (!dust.noLight)
            {
                Vector3 lightColor;
                //if (dust.shader != null)
                //    lightColor = DyeHelper.ModifyLight(dust.shader, dust.color.ToVector3() * 0.5f);
                //else
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