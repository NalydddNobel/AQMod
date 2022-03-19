using AQMod.Content.World;
using AQMod.Content.World.Events;
using AQMod.Effects;
using AQMod.NPCs;
using AQMod.Tiles.CrabCrevice;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod
{
    public class AQWorld : ModWorld
    {
        public static class Hooks
        {
            public static bool EclipsePreventionFailed;
            public static bool BloodMoonPreventionFailed;

            internal static void Apply()
            {
                EclipsePreventionFailed = false;
                BloodMoonPreventionFailed = false;
                IL.Terraria.Main.UpdateTime += DisableSomeNaturalEvents;
                On.Terraria.Main.UpdateSundial += Main_UpdateSundial;
                On.Terraria.Main.UpdateWeather += Main_UpdateWeather;
            }

            internal static void Unload()
            {
                IL.Terraria.Main.UpdateTime -= DisableSomeNaturalEvents;
            }

            private static void DisableSomeNaturalEvents(ILContext il)
            {
                var cursor = new ILCursor(il);

                // Prevent Eclipses
                if (!cursor.TryGotoNext(i => i.MatchCall(typeof(Main), nameof(Main.checkXMas))))
                {
                    EclipsePreventionFailed = true;
                    return;
                }

                if (!cursor.TryGotoNext(i => i.MatchLdsfld(typeof(NPC), nameof(NPC.downedMechBossAny))))
                {
                    EclipsePreventionFailed = true;
                    return;
                }

                if (!cursor.TryGotoNext(i => i.MatchStsfld(typeof(Main), nameof(Main.eclipse))))
                {
                    EclipsePreventionFailed = true;
                    return;
                }
                if (!cursor.TryGotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.eclipse))))
                {
                    EclipsePreventionFailed = true;
                    return;
                }

                cursor.EmitDelegate<Action>(() =>
                {
                    if (MiscWorldInfo.eclipseDisabled && Main.eclipse)
                    {
                        Main.eclipse = false;
                        MiscWorldInfo.eclipsesPrevented++;
                    }
                });

                // Prevent Blood Moons
                if (!cursor.TryGotoNext(i => i.MatchCall(typeof(NPC), nameof(NPC.setFireFlyChance))))
                {
                    BloodMoonPreventionFailed = true;
                    return;
                }

                if (!cursor.TryGotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.fastForwardTime))))
                {
                    BloodMoonPreventionFailed = true;
                    return;
                }

                for (int k = 0; k < 2; k++)
                {
                    if (!cursor.TryGotoNext(i => i.MatchLdsfld(typeof(WorldGen), nameof(WorldGen.spawnEye))))
                    {
                        BloodMoonPreventionFailed = true;
                        return;
                    }
                }

                if (!cursor.TryGotoNext(i => i.MatchStsfld(typeof(Main), nameof(Main.bloodMoon))))
                {
                    BloodMoonPreventionFailed = true;
                    return;
                }
                if (!cursor.TryGotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.bloodMoon))))
                {
                    BloodMoonPreventionFailed = true;
                    return;
                }

                var labels = cursor.IncomingLabels;
                cursor.Remove();
                int index = cursor.Index;
                cursor.EmitDelegate<Action>(() =>
                {
                    if (MiscWorldInfo.bloodMoonDisabled && Main.bloodMoon)
                    {
                        Main.bloodMoon = false;
                        MiscWorldInfo.bloodMoonsPrevented++;
                    }
                });
                cursor.Emit(OpCodes.Ldsfld, typeof(Main).GetField("bloodMoon", BindingFlags.Public | BindingFlags.Static));
                cursor.Index = index;
                foreach (var l in labels)
                {
                    cursor.MarkLabel(l);
                }
            }

            private static void Main_UpdateWeather(On.Terraria.Main.orig_UpdateWeather orig, Main self, GameTime gameTime)
            {
                if (GaleStreams.EndEvent)
                {
                    Main.windSpeedSet += -Math.Sign(Main.windSpeedSet) / 100f;
                    if (Main.windSpeedSet.Abs() < 0.1f)
                    {
                        GaleStreams.EndEvent = false;
                    }
                    Main.windSpeedTemp = Main.windSpeedSet;
                    return;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && (Main.netMode == NetmodeID.Server || !Main.gameMenu)
                    && GaleStreams.IsActive)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active && !Main.player[i].dead && GaleStreams.EventActive(Main.player[i]))
                        {
                            Main.cloudLimit = 200; // prevents the wind speed from naturally changing during the Gale Streams event
                            if (Main.windSpeed < Main.windSpeedSet)
                            {
                                Main.windSpeed += 0.001f * Main.dayRate;
                                if (Main.windSpeed > Main.windSpeedSet)
                                {
                                    Main.windSpeed = Main.windSpeedSet;
                                }
                            }
                            else if (Main.windSpeed > Main.windSpeedSet)
                            {
                                Main.windSpeed -= 0.001f * Main.dayRate;
                                if (Main.windSpeed < Main.windSpeedSet)
                                {
                                    Main.windSpeed = Main.windSpeedSet;
                                }
                            }
                            Main.weatherCounter -= Main.dayRate;
                            if (Main.weatherCounter <= 0)
                            {
                                Main.weatherCounter = Main.rand.Next(3600, 18000);
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.WorldData);
                                }
                            }
                            return;
                        }
                    }
                }
                if (Main.windSpeedSet.Abs() > 1f)
                {
                    Main.windSpeedSet += -Math.Sign(Main.windSpeedSet) / 100f;
                    Main.windSpeedTemp = Main.windSpeedSet;
                }
                orig(self, gameTime);
            }

            private static void Main_UpdateSundial(On.Terraria.Main.orig_UpdateSundial orig)
            {
                orig();
                Main.dayRate += dayrate;
            }
        }

        public static int nobleMushroomsNearby;
        public static int dayrate;

        public override void Initialize()
        {
            if (!Main.dayTime)
                SkyGlimmerEvent.InitNight();
            EventProgressBarLoader.ActiveBar = 255;
            NoHitting.CurrentlyDamaged = new List<byte>();
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            nobleMushroomsNearby = tileCounts[ModContent.TileType<NobleMushroomsNew>()];
        }
    }
}