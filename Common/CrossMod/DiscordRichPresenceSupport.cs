using AQMod.Common.Configuration;
using AQMod.Content.World.Events;
using AQMod.NPCs.Bosses;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Common.CrossMod
{
    internal static class DiscordRichPresenceSupport
    {
        public static void AddSupport()
        {
            try
            {
                var config = ModContent.GetInstance<DiscordRichPresenceConfig>();
                if (config != null && config.enableDiscordRichPresence)
                {
                    var drp = AQMod.discordRP.mod;
                    if (drp != null)
                    {
                        drp.Call("AddClient", "930160636749574155", "mod_aequus");
                        drp.Call("AddBoss", new List<int> { ModContent.NPCType<JerryCrabson>() }, "Crabson", "boss_crabson", 2f, "mod_aequus");
                        drp.Call("AddBoss", new List<int> { ModContent.NPCType<OmegaStarite>() }, "Omega Starite", "boss_omegastarite", 10f, "mod_aequus");

                        if (config.miniBossRichPresence)
                        {
                            drp.Call("AddBoss", new List<int> { ModContent.NPCType<RedSprite>() }, "Red Sprite", "boss_redsprite", 9f, "mod_aequus");
                            drp.Call("AddBoss", new List<int> { ModContent.NPCType<SpaceSquid>() }, "Space Squid", "boss_spacesquid", 9f, "mod_aequus");
                        }

                        if (config.eventRichPresence)
                        {
                            drp.Call("AddBiome", (Func<bool>)(() => Main.LocalPlayer.Biomes().zoneCrabCrevice), "Crab Crevice",
                                "biome_crabseason", 50f, "mod_aequus");

                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!Glimmer.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                int tileDistance = Glimmer.GetTileDistanceUsingPlayer(Main.LocalPlayer);
                                if (tileDistance > Glimmer.SuperStariteDistance)
                                {
                                    return false;
                                }
                                return tileDistance <= Glimmer.MaxDistance;
                            }),
                                "the edge of the Glimmer",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!Glimmer.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                int tileDistance = Glimmer.GetTileDistanceUsingPlayer(Main.LocalPlayer);
                                if (tileDistance > Glimmer.HyperStariteDistance)
                                {
                                    return false;
                                }
                                return tileDistance <= Glimmer.SuperStariteDistance;
                            }),
                                "Glimmer Event! Super Starites!",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!Glimmer.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                int tileDistance = Glimmer.GetTileDistanceUsingPlayer(Main.LocalPlayer);
                                if (tileDistance > Glimmer.UltraStariteDistance)
                                {
                                    return false;
                                }
                                return tileDistance <= Glimmer.HyperStariteDistance;
                            }),
                                "Glimmer Event! Hyper Starites!",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() =>
                            {
                                if (!Glimmer.IsGlimmerEventCurrentlyActive())
                                {
                                    return false;
                                }
                                return Glimmer.GetTileDistanceUsingPlayer(Main.LocalPlayer) <= Glimmer.UltraStariteDistance;
                            }),
                                "the Ultimate Sword...",
                                "biome_glimmerevent", 50f, "mod_aequus");
                            drp.Call("AddBiome", (Func<bool>)(() => Main.LocalPlayer.Biomes().zoneDemonSiege),
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
