using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts
{
    public class UltimaDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(255, 255, 255, 20);

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.9f;
            float velo = dust.velocity.Length();
            dust.rotation += velo * 0.0314f;
            dust.scale -= 0.05f - velo / 1000f;
            if (dust.scale <= 0.1f)
            {
                dust.active = false;
            }
            if (!dust.noLight)
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);
            }
            dust.position += dust.velocity;
            return false;
        }
    }
}