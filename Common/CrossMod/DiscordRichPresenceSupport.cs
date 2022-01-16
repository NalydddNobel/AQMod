using AQMod.Common.Configuration;
using AQMod.Content.World.Events;
using AQMod.Content.World.Events.DemonSiege;
using AQMod.Content.World.Events.GaleStreams;
using AQMod.Content.World.Events.GlimmerEvent;
using AQMod.NPCs.Boss;
using AQMod.NPCs.Boss.Crabson;
using AQMod.NPCs.Monsters.GaleStreams;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    internal static class DiscordRichPresenceSupport
    {
        public static Mod GetMod() => ModLoader.GetMod("DiscordRP");

        public static void AddSupport(AQMod aQMod)
        {
            try
            {
                var config = ModContent.GetInstance<DiscordRichPresenceConfig>();
                if (config != null && config.enableDiscordRichPresence)
                {
                    var drp = GetMod();
                    if (drp != null)
                    {
                        drp.Call("AddClient", "930160636749574155", "mod_aequus");
                        drp.Call("AddBoss", new List<int> { ModContent.NPCType<JerryCrabson>() }, "Jerry Crabson", "boss_crabson", 2f, "mod_aequus");
                        drp.Call("AddBoss", new List<int> { ModContent.NPCType<OmegaStarite>() }, "Omega Starite", "boss_omegastarite", 10f, "mod_aequus");

                        if (config.miniBossRichPresence)
                        {
                            drp.Call("AddBoss", new List<int> { ModContent.NPCType<RedSprite>() }, "Red Sprite", "boss_redsprite", 9f, "mod_aequus");
                            drp.Call("AddBoss", new List<int> { ModContent.NPCType<SpaceSquid>() }, "Space Squid", "boss_spacesquid", 9f, "mod_aequus");
                        }

                        if (config.eventRichPresence)
                        {
                            drp.Call("AddBiome", (Func<bool>)(() => CrabSeason.InActiveZone(Main.LocalPlayer)), "Crab Season",
                                "biome_crabseason", 50f, "mod_aequus");

                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!GlimmerEvent.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                int tileDistance = GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer);
                                if (tileDistance > GlimmerEvent.SuperStariteDistance)
                                {
                                    return false;
                                }
                                return tileDistance <= GlimmerEvent.MaxDistance;
                            }),
                                "the edge of the Glimmer",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!GlimmerEvent.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                int tileDistance = GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer);
                                if (tileDistance > GlimmerEvent.HyperStariteDistance)
                                {
                                    return false;
                                }
                                return tileDistance <= GlimmerEvent.SuperStariteDistance;
                            }),
                                "Glimmer Event! Super Starites!",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!GlimmerEvent.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                int tileDistance = GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer);
                                if (tileDistance > GlimmerEvent.UltraStariteDistance)
                                {
                                    return false;
                                }
                                return tileDistance <= GlimmerEvent.HyperStariteDistance;
                            }),
                                "Glimmer Event! Hyper Starites!",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!GlimmerEvent.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                return GlimmerEvent.GetTileDistanceUsingPlayer(Main.LocalPlayer) <= GlimmerEvent.UltraStariteDistance;
                            }),
                                "the Ultimate Sword...",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() => DemonSiege.CloseEnoughToDemonSiege(Main.LocalPlayer)),
                                "Demon Siege",
                                "biome_demonsiege", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() => GaleStreams.EventActive(Main.LocalPlayer)),
                                "Gale Streams",
                                "biome_galestreams", 50f, "mod_aequus");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AQMod.GetInstance().Logger.Warn("There was an error when loading Discord Rich Presence support!", ex);
            }
        }
    }
}
