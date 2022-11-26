using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles.Dusts
{
    public class CarpenterSurpriseDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = (Texture2D?.Value?.Bounds).GetValueOrDefault();
        }

        public override bool Update(Dust dust)
        {
            return true;
        }

        public override bool MidUpdate(Dust dust)
        {
            dust.rotation = 0f;
            if (dust.velocity.Y > 0f)
            {
                dust.velocity *= 0.9f;
                dust.scale *= 0.98f;
            }
            return false;
        }
    }
}