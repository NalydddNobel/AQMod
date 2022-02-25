using AQMod.Assets;
using AQMod.Content.Seasonal.Christmas;
using AQMod.Content.World.Events;
using AQMod.Effects;
using AQMod.NPCs;
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
        public bool zoneBoss;

        private void UpdateBiomes_Crabs(int x, int y)
        {
            zoneCrabCrevice = Main.tile[x, y].wall == ModContent.WallType<OceanRavineWall>() ||
                Main.tile[x, y].wall == ModContent.WallType<PetrifiedWoodWall>();
            zoneCrabSeason = zoneCrabCrevice && player.position.Y < Main.worldSurface * 16f;
        }
        private void UpdateBiomes_GlimmerEvent()
        {
            if ((player.ZoneOverworldHeight || player.ZoneSkyHeight) && Glimmer.IsGlimmerEventCurrentlyActive())
            {
                int glimmerTileDistance = Glimmer.Distance(player);
                zoneGlimmerEvent = glimmerTileDistance < Glimmer.MaxDistance;
                zoneGlimmerEventLayer = (byte)(zoneGlimmerEvent ? Glimmer.FindLayer(glimmerTileDistance) : 255);
            }
            else
            {
                zoneGlimmerEvent = false;
                zoneGlimmerEventLayer = 255;
            }
        }
        public override void UpdateBiomes()
        {
            try
            {
                int x = (int)(player.position.X + player.width / 2) / 16;
                int y = (int)(player.position.Y + player.height / 2) / 16;
                zoneAtmosphericCurrentsEvent = Main.hardMode && player.ZoneSkyHeight && Main.windSpeed > 30f;

                UpdateBiomes_Crabs(x, y);
                UpdateBiomes_GlimmerEvent();

                zoneDemonSiege = DemonSiege.IsActive ? player.Distance(new Vector2(DemonSiege.X * 16f, DemonSiege.Y * 16f)) < 2000f : false;
                zoneBoss = NPCSpawnChanger.SpawnRate_CheckBosses();
            }
            catch
            {

            }
        }

        private void UpdateBiomeVisuals_Starite()
        {
            if (Glimmer.omegaStarite == -1)
                Glimmer.omegaStarite = (short)NPC.FindFirstNPC(ModContent.NPCType<OmegaStarite>());
            bool glimmerEvent = (Glimmer.IsGlimmerEventCurrentlyActive() || Glimmer.omegaStarite != -1) && Main.screenPosition.Y < (Main.worldSurface * 16f - Main.screenHeight);
            AQUtils.UpdateSky(glimmerEvent, SkyGlimmerEvent.Name);

            if (glimmerEvent && Glimmer.omegaStarite == -1)
            {
                float intensity = 0f;
                float distance = (Main.player[Main.myPlayer].position.X - (Glimmer.tileX * 16f + 8f)).Abs();
                if (distance < 6400f)
                {
                    intensity += 1f - distance / 6400f;
                }

                var filter = LegacyEffectCache.f_Vignette;
                var shader = LegacyEffectCache.f_Vignette.GetShader();
                shader.UseIntensity(intensity * 1.25f);
                if (!LegacyEffectCache.f_Vignette.IsActive())
                    Filters.Scene.Activate(LegacyEffectCache.fn_Vignette);
            }
            else
            {
                if (LegacyEffectCache.f_Vignette.IsActive())
                    Filters.Scene.Deactivate(LegacyEffectCache.fn_Vignette);
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
                 zoneDemonSiege == otherBiomes.zoneDemonSiege &&
                 zoneBoss == otherBiomes.zoneBoss;
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
            otherBiomes.zoneBoss = zoneBoss;
        }

        public override void SendCustomBiomes(BinaryWriter writer)
        {
            writer.Write(zoneAtmosphericCurrentsEvent);
            writer.Write(zoneCrabCrevice);
            writer.Write(zoneCrabSeason);
            writer.Write(zoneGlimmerEvent);
            writer.Write(zoneGlimmerEventLayer);
            writer.Write(zoneDemonSiege);
            writer.Write(zoneBoss);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader)
        {
            zoneAtmosphericCurrentsEvent = reader.ReadBoolean();
            zoneCrabCrevice = reader.ReadBoolean();
            zoneCrabSeason = reader.ReadBoolean();
            zoneGlimmerEvent = reader.ReadBoolean();
            zoneGlimmerEventLayer = reader.ReadByte();
            zoneDemonSiege = reader.ReadBoolean();
            zoneBoss = reader.ReadBoolean();
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
                    if (!player.ZoneDesert && Glimmer.IsGlimmerEventCurrentlyActive())
                    {
                        if (Glimmer.Distance(player) < Glimmer.UltraStariteDistance)
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