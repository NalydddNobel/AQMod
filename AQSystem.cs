using AQMod.Content.World.Events;
using AQMod.Effects;
using AQMod.NPCs;
using AQMod.NPCs.Friendly;
using AQMod.Tiles.Nature.CrabCrevice;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod
{
    public class AQSystem : ModWorld
    {
        public static class Hooks
        {
            internal static void Main_UpdateWeather(On.Terraria.Main.orig_UpdateWeather orig, Main self, GameTime gameTime)
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

            internal static void Main_UpdateSundial(On.Terraria.Main.orig_UpdateSundial orig)
            {
                orig();
                Main.dayRate += DayrateIncrease;
            }
        }

        public static int NobleMushroomsCount { get; private set; }

        public static int DayrateIncrease { get; set; }

        public static bool UpdatingTime { get; internal set; }
        public static bool CosmicanonActive { get; internal set; }

        public override void Initialize()
        {
            if (!Main.dayTime)
                SkyGlimmerEvent.InitNight();
            Robster.Initalize();
            EventProgressBarLoader.ActiveBar = 255;
            NPCNoHit.CurrentlyDamaged = new List<byte>();
        }

        public override void TileCountsAvailable(int[] tileCounts)
        {
            NobleMushroomsCount = tileCounts[ModContent.TileType<NobleMushrooms>()] + tileCounts[ModContent.TileType<NobleMushroomsNew>()];
        }

        public override TagCompound Save()
        {
            var tag = new TagCompound();
            Robster.Save(tag);
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            Robster.Load(tag);
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Robster.QuestsCompleted);
            if (Robster.ActiveQuest != null)
            {
                writer.Write(true);
                writer.Write(Robster.TargetNPC);
                writer.Write(Robster.ActiveQuest.Key);
                writer.Write(Robster.ActiveQuest.type);
                writer.Write(Robster.ActiveQuest.location.X);
                writer.Write(Robster.ActiveQuest.location.Y);
                Robster.ActiveQuest.NetSend(writer);
            }
            else
            {
                writer.Write(false);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            Robster.QuestsCompleted = reader.ReadInt32();
            if (reader.ReadBoolean())
            {
                Robster.TargetNPC = reader.ReadInt32();
                string key = reader.ReadString();
                Robster.ActiveQuest = Robster.RegisteredQuests.Find((h) => h.Key == key);
                Robster.ActiveQuest.type = reader.ReadByte();
                Robster.ActiveQuest.location.X = reader.ReadInt32();
                Robster.ActiveQuest.location.Y = reader.ReadInt32();
                Robster.ActiveQuest.NetRecieve(reader);
            }
        }
    }
}