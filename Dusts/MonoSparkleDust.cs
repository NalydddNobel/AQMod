using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Dusts
{
    public sealed class MonoSparkleDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return new Color(lightColor.ToVector3() * dust.color.ToVector3())
            {
                A = 25
            };
        }

        public override bool Update(Dust dust)
        {
            if (dust.scale > 10f)
            {
                dust.active = false;
            }
            dust.position += dust.velocity;
            dust.rotation += dust.velocity.X * 0.5f;
            if (!dust.noLight)
            {
                Lighting.AddLight(dust.position, dust.color.ToVector3() * (dust.scale > 1f ? 1f : dust.scale));
            }
            if (dust.noGravity)
            {
                dust.velocity *= 0.93f;
                if (dust.velocity.Length() < 1f)
                {
                    dust.noGravity = false;
                }
                if (dust.fadeIn == 0f)
                {
                    dust.scale += 0.0025f;
                }
            }
            else
            {
                dust.velocity *= 0.95f;
                dust.scale -= 0.01f;
            }
            var tileCoords = dust.position.ToTileCoordinates();
            if (WorldGen.InWorld(tileCoords.X, tileCoords.Y, 10) && WorldGen.SolidTile(Framing.GetTileSafely(tileCoords.X, tileCoords.Y)) && dust.fadeIn == 0f && !dust.noGravity)
            {
                dust.scale *= 0.9f;
                dust.velocity *= 0.25f;
            }
            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }
            return false;
        }
    }
}