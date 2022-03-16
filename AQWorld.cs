using AQMod.Content.World;
using AQMod.Content.World.Events;
using AQMod.Effects;
using AQMod.NPCs;
using AQMod.NPCs.Friendly;
using AQMod.Tiles.Nature.CrabCrevice;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
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
            internal static void Apply()
            {
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
                cursor.GotoNext(i => i.MatchCall(typeof(Main), nameof(Main.checkXMas)));

                cursor.GotoNext(i => i.MatchLdsfld(typeof(NPC), nameof(NPC.downedMechBossAny)));

                cursor.GotoNext(i => i.MatchStsfld(typeof(Main), nameof(Main.eclipse)));
                cursor.GotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.eclipse)));

                cursor.EmitDelegate<Action>(() =>
                {
                    if (MiscWorldInfo.eclipseDisabled && Main.eclipse)
                    {
                        Main.eclipse = false;
                        MiscWorldInfo.eclipsesPrevented++;
                    }
                });

                // Prevent Blood Moons
                cursor.GotoNext(i => i.MatchCall(typeof(NPC), nameof(NPC.setFireFlyChance)));

                cursor.GotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.fastForwardTime)));

                for (int k = 0; k < 2; k++)
                    cursor.GotoNext(i => i.MatchLdsfld(typeof(WorldGen), nameof(WorldGen.spawnEye)));

                cursor.GotoNext(i => i.MatchStsfld(typeof(Main), nameof(Main.bloodMoon)));
                cursor.GotoNext(i => i.MatchLdsfld(typeof(Main), nameof(Main.bloodMoon)));

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
                Main.dayRate += DayrateIncrease;
            }
        }

        public static int NobleMushroomsCount { get; private set; }

        public static int DayrateIncrease { get; set; }

        public static bool UpdatingTime { get; internal set; }

        public override void Initialize()
        {
            if (!Main.dayTime)
                SkyGlimmerEvent.InitNight();
            EventProgressBarLoader.ActiveBar = 255;
            NoHitting.CurrentlyDamaged = new List<byte>();
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()] + tileCounts[ModContent.TileType<NobleMushroomsNew>()];
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Robster.QuestsCompleted);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Robster.QuestsCompleted = reader.ReadInt32();
        }
    }
}