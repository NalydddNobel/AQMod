using AQMod.Assets;
using AQMod.Content.Seasonal.Christmas;
using AQMod.Content.World;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.Effects;
using AQMod.NPCs.Bosses;
using AQMod.Tiles.Walls;
using Microsoft.Xna.Framework;
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
        public bool zoneDemonSiege;

        private void UpdateBiomes_GlimmerEvent()
        {
            if (EventGlimmer.IsGlimmerEventCurrentlyActive())
            {
                int glimmerTileDistance = EventGlimmer.GetTileDistanceUsingPlayer(player);
                zoneGlimmerEvent = glimmerTileDistance < EventGlimmer.MaxDistance;
                zoneGlimmerEventLayer = (byte)EventGlimmer.GetLayerIndexThroughTileDistance(glimmerTileDistance);
            }
            else
            {
                zoneGlimmerEvent = false;
                zoneGlimmerEventLayer = 255;
            }
        }
        public override void UpdateBiomes()
        {
            int x = (int)(player.position.X + player.width / 2) / 16;
            int y = (int)(player.position.Y + player.height / 2) / 16;
            zoneAtmosphericCurrentsEvent = player.ZoneSkyHeight && Main.windSpeed > 30f;
            zoneCrabCrevice = Main.tile[x, y].wall == ModContent.WallType<OceanRavineWall>() ||
                Main.tile[x, y].wall == ModContent.WallType<PetrifiedWoodWall>();
            zoneCrabSeason = zoneCrabCrevice && player.position.Y < Main.worldSurface * 16f;

            UpdateBiomes_GlimmerEvent();

            zoneDemonSiege = EventDemonSiege.IsActive ? player.Distance(new Vector2(EventDemonSiege.X * 16f, EventDemonSiege.Y * 16f)) < 2000f : false;
        }

        private void UpdateBiomeVisuals_Starite()
        {
            if (EventGlimmer.omegaStarite == -1)
                EventGlimmer.omegaStarite = (short)NPC.FindFirstNPC(ModContent.NPCType<OmegaStarite>());
            bool glimmerEvent = (EventGlimmer.IsGlimmerEventCurrentlyActive() || EventGlimmer.omegaStarite != -1) && Main.screenPosition.Y < (Main.worldSurface * 16f - Main.screenHeight);
            AQUtils.UpdateSky(glimmerEvent, SkyGlimmerEvent.Name);

            if (glimmerEvent && EventGlimmer.omegaStarite == -1)
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
                    Filters.Scene.Activate(EffectCache.fn_Vignette);
            }
            else
            {
                if (EffectCache.f_Vignette.IsActive())
                    Filters.Scene.Deactivate(EffectCache.fn_Vignette);
            }
        }
        public override void UpdateBiomeVisuals()
        {
            UpdateBiomeVisuals_Starite();
        }

        public override bool CustomBiomesMatch(Player other)
        {
            var otherBiomes = other.Biomes();
            return zoneAtmosphericCurrentsEvent == otherBiomes.zoneAtmosphericCurrentsEvent &&
                 zoneCrabCrevice == otherBiomes.zoneCrabCrevice &&
                 zoneCrabSeason == otherBiomes.zoneCrabSeason &&
                 zoneGlimmerEvent == otherBiomes.zoneGlimmerEvent &&
                 zoneGlimmerEventLayer == otherBiomes.zoneGlimmerEventLayer &&
                 zoneDemonSiege == otherBiomes.zoneDemonSiege;
        }

        public override void CopyCustomBiomesTo(Player other)
        {
            var otherBiomes = other.Biomes();
            otherBiomes.zoneAtmosphericCurrentsEvent = zoneAtmosphericCurrentsEvent;
            otherBiomes.zoneCrabCrevice = zoneCrabCrevice;
            otherBiomes.zoneCrabSeason = zoneCrabSeason;
            otherBiomes.zoneGlimmerEvent = zoneGlimmerEvent;
            otherBiomes.zoneGlimmerEventLayer = zoneGlimmerEventLayer;
            otherBiomes.zoneDemonSiege = zoneDemonSiege;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            writer.Write(zoneAtmosphericCurrentsEvent);
            writer.Write(zoneCrabCrevice);
            writer.Write(zoneCrabSeason);
            writer.Write(zoneGlimmerEvent);
            writer.Write(zoneGlimmerEventLayer);
            writer.Write(zoneDemonSiege);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            zoneAtmosphericCurrentsEvent = reader.ReadBoolean();
            zoneCrabCrevice = reader.ReadBoolean();
            zoneCrabSeason = reader.ReadBoolean();
            zoneGlimmerEvent = reader.ReadBoolean();
            zoneGlimmerEventLayer = reader.ReadByte();
            zoneDemonSiege = reader.ReadBoolean();
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