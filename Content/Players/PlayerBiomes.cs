using AQMod.Assets;
using AQMod.Common.Configuration;
using AQMod.Content.Seasonal.Christmas;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.Walls;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public sealed class PlayerBiomes : ModPlayer
    {
        public bool zoneAtmosphericCurrentsEvent;
        public bool zoneCrabSeason;
        public bool zoneCrabCrevice;

        public override void UpdateBiomes()
        {
            int x = (int)(player.position.X + player.width / 2) / 16;
            int y = (int)(player.position.Y + player.height / 2) / 16;
            zoneAtmosphericCurrentsEvent = player.ZoneSkyHeight && Main.windSpeed > 30f;
            zoneCrabCrevice = Main.tile[x, y].wall == ModContent.WallType<OceanRavineWall>();
        }

        public override void UpdateBiomeVisuals()
        {
            bool glimmerEvent = (GlimmerEvent.IsGlimmerEventCurrentlyActive() || OmegaStariteScenes.OmegaStariteIndexCache != -1) && Main.screenPosition.Y < Main.worldSurface * 16f + Main.screenHeight;
            AQUtils.UpdateSky(glimmerEvent, GlimmerEventSky.Name);

            if (glimmerEvent && OmegaStariteScenes.OmegaStariteIndexCache == -1 && ModContent.GetInstance<StariteConfig>().UltimateSwordVignette)
            {
                float intensity = 0f;
                float distance = (Main.player[Main.myPlayer].position.X - (GlimmerEvent.tileX * 16f + 8f)).Abs();
                if (distance < 6400f)
                {
                    intensity += 1f - distance / 6400f;
                }

                var filter = EffectCache.f_Vignette;
                var shader = EffectCache.f_Vignette.GetShader();
                shader.UseIntensity(intensity * 1.25f);
                if (!EffectCache.f_Vignette.IsActive())
                {
                    Filters.Scene.Activate(EffectCache.fn_Vignette);
                }
            }
            else
            {
                if (EffectCache.f_Vignette.IsActive())
                    Filters.Scene.Deactivate(EffectCache.fn_Vignette);
            }
        }

        public override Texture2D GetMapBackgroundImage()
        {
            if (XmasSeeds.XmasWorld && WorldGen.gen)
            {
                return ModContent.GetTexture("Terraria/MapBG12");
            }
            if (!player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneHoly && !player.ZoneJungle)
            {
                if (player.position.Y < Main.worldSurface * 16f)
                {
                    if (!player.ZoneDesert && GlimmerEvent.IsGlimmerEventCurrentlyActive())
                    {
                        if (GlimmerEvent.GetTileDistanceUsingPlayer(player) < GlimmerEvent.UltraStariteDistance)
                        {
                            return ModContent.GetTexture(TexturePaths.MapBackgrounds + "ultimatesword");
                        }
                        return ModContent.GetTexture(TexturePaths.MapBackgrounds + "glimmerevent");
                    }
                    else if (zoneCrabSeason || zoneCrabCrevice)
                    {
                        return ModContent.GetTexture(TexturePaths.MapBackgrounds + "crabseason");
                    }
                }
                else if (zoneCrabCrevice)
                {
                    return ModContent.GetTexture(TexturePaths.MapBackgrounds + "crabcrevice");
                }
            }
            return null;
        }
    }
}