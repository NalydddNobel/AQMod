using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Particles.Dusts
{
    public class GhostDrainDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;

        public override bool Update(Dust dust)
        {
            if (dust.customData == null)
            {
                dust.active = false;
                return false;
            }
            if (!dust.noLight)
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);
            }
            var player = (Player)dust.customData;
            var diff = player.MountedCenter - dust.position;
            float lerpValue = Math.Clamp(1f - 1f * dust.scale, 0.05f, 0.33f);
            if (diff.Length() < 120f)
            {
                lerpValue = Math.Min(lerpValue * 1.25f, 1f);
            }
            if (diff.Length() < 60f)
            {
                lerpValue = Math.Min(lerpValue * 2f, 1f);
            }
            dust.velocity = Vector2.Lerp(dust.velocity, Vector2.Normalize(diff) * Math.Max(diff.Length() / 15f * Math.Clamp(lerpValue * 3f, 1f, 15f), 2f), lerpValue);
            dust.position += dust.velocity;
            dust.scale -= 0.02f;
            if (diff.Length() < 20f)
            {
                dust.scale -= 0.02f;
            }
            if (dust.scale <= 0.1f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}