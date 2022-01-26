using AQMod.Assets;
using AQMod.Content.Seasonal.Christmas;
using AQMod.Content.World;
using AQMod.Effects;
using AQMod.Walls;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace AQMod.Content.Players
{
    public sealed class PlayerBiomes : ModPlayer
    {
        public bool zoneAtmosphericCurrentsEvent;
        public bool zoneCrabCrevice;
        public bool zoneCrabSeason;
        public bool zoneGlimmerEvent;
        public byte zoneGlimmerEventLayer;

        public override void UpdateBiomes()
        {
            int x = (int)(player.position.X + player.width / 2) / 16;
            int y = (int)(player.position.Y + player.height / 2) / 16;
            zoneAtmosphericCurrentsEvent = player.ZoneSkyHeight && Main.windSpeed > 30f;
            zoneCrabCrevice = Main.tile[x, y].wall == ModContent.WallType<OceanRavineWall>() ||
                Main.tile[x, y].wall == ModContent.WallType<PetrifiedWoodWall>();
        }

        public override void UpdateBiomeVisuals()
        {
            bool glimmerEvent = (EventGlimmer.IsGlimmerEventCurrentlyActive() || EventGlimmer.OmegaStarite != -1) && Main.screenPosition.Y < Main.worldSurface * 16f + Main.screenHeight;
            AQUtils.UpdateSky(glimmerEvent, SkyGlimmerEvent.Name);

            if (glimmerEvent && EventGlimmer.OmegaStarite == -1)
            {
                float intensity = 0f;
                float distance = (Main.player[Main.myPlayer].position.X - (EventGlimmer.tileX * 16f + 8f)).Abs();
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

        public override bool CustomBiomesMatch(Player other)
        {
            var otherBiomes = other.Biomes();
            return zoneAtmosphericCurrentsEvent == otherBiomes.zoneAtmosphericCurrentsEvent &&
                 zoneCrabCrevice == otherBiomes.zoneCrabCrevice &&
                 zoneCrabSeason == otherBiomes.zoneCrabSeason &&
                 zoneGlimmerEvent == otherBiomes.zoneGlimmerEvent;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            var otherBiomes = other.Biomes();
            otherBiomes.zoneAtmosphericCurrentsEvent = zoneAtmosphericCurrentsEvent;
            otherBiomes.zoneCrabCrevice = zoneCrabCrevice;
            otherBiomes.zoneCrabSeason = zoneCrabSeason;
            otherBiomes.zoneGlimmerEvent = zoneGlimmerEvent;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            writer.Write(zoneAtmosphericCurrentsEvent);
            writer.Write(zoneCrabCrevice);
            writer.Write(zoneCrabSeason);
            writer.Write(zoneGlimmerEvent);
            writer.Write(zoneGlimmerEventLayer);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            zoneAtmosphericCurrentsEvent = reader.ReadBoolean();
            zoneCrabCrevice = reader.ReadBoolean();
            zoneCrabSeason = reader.ReadBoolean();
            zoneGlimmerEvent = reader.ReadBoolean();
            zoneGlimmerEventLayer = reader.ReadByte();
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
                    if (!player.ZoneDesert && EventGlimmer.IsGlimmerEventCurrentlyActive())
                    {
                        if (EventGlimmer.GetTileDistanceUsingPlayer(player) < EventGlimmer.UltraStariteDistance)
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