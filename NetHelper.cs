using AQMod.Common;
using AQMod.Content;
using AQMod.Content.LegacyWorldEvents.DemonSiege;
using AQMod.Content.World.Events.GlimmerEvent;
using System.IO;
using Terraria;
using Terraria.ID;

namespace AQMod
{
    public static class NetHelper
    {
        public static class PacketType
        {
            public const ushort PreventedBloodMoon = 0;
            public const ushort PreventedGlimmer = 1;
            public const ushort PreventedEclipse = 2;
            public const ushort UpdateWindSpeeds = 3;

            public const ushort ActivateGlimmerEvent = 500;
            public const ushort RequestOmegaStarite = 1000;
            public const ushort RequestDemonSiege = 2000;
            public const ushort RequestDemonSiegeEnemy = 2001;

            public const ushort Flag_ExporterIntroduction = 10000;
            public const ushort Flag_PhysicistIntroduction = 10001;
            public const ushort Flag_AirHunterIntroduction = 10002;
        }

        internal class PacketInvokerType
        {
            public readonly ushort Type;

            public PacketInvokerType(ushort type)
            {
                Type = type;
            }

            public override int GetHashCode()
            {
                return Type;
            }

            public static PacketInvokerType GetType(ushort type)
            {
                return null;
            }
        }

        #region Demon Siege
        public static void RequestDemonSiege(int x, int y, int plr, Item item)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.RequestDemonSiege);
            p.Write(x);
            p.Write(y);
            p.Write(plr);
            p.Write(item.netID);
            p.Write(item.stack);
            p.Write(item.prefix);
            if (item.type > Main.maxItemTypes)
            {
                item.modItem.NetSend(p);
            }
            p.Send();
        }
        public static void RequestDemonSiegeEnemy(int x, int y)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.RequestDemonSiege);
            p.Write(x);
            p.Write(y);
            p.Send();
        }
        #endregion

        #region Glimmer Event 
        public static void RequestOmegaStarite()
        {
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.RequestOmegaStarite);
            p.Send();
        }

        public static void ActivateGlimmerEvent()
        {
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.ActivateGlimmerEvent);
            p.Write(GlimmerEvent.tileX);
            p.Write(GlimmerEvent.tileY);
            p.Send();
        }
        #endregion

        #region Misc
        public static void FlagSet(ushort type)
        {
            var p = AQMod.Instance.GetPacket();
            p.Write(type);
            p.Send();
        }
        public static void UpdateWindSpeeds()
        {
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.UpdateWindSpeeds);
            p.Write(Main.windSpeedSet);
            p.Write(Main.windSpeedSpeed);
            p.Write(Main.windSpeedTemp);
            p.Send();
        }

        public static void PreventedBloodMoon()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.PreventedBloodMoon);
            p.Write(CosmicanonCounts.BloodMoonsPrevented);
            p.Send();
        }

        public static void PreventedGlimmer()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.PreventedGlimmer);
            p.Write(CosmicanonCounts.GlimmersPrevented);
            p.Send();
        }

        public static void PreventedEclipse()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.Instance.GetPacket();
            p.Write(PacketType.PreventedEclipse);
            p.Write(CosmicanonCounts.EclipsesPrevented);
            p.Send();
        }
        #endregion

        public static void ReadPacket(BinaryReader reader, int sender)
        {
            ushort messageID = reader.ReadUInt16();

            AQMod.Debug.DebugLogger? l = null;
            if (AQMod.Debug.LogNetcode)
            {
                l = AQMod.Debug.GetDebugLogger();
                l.Value.Log("Message ID: " + messageID);
            }

            switch (messageID)
            {
                case PacketType.PreventedBloodMoon:
                    {
                        l?.Log("Old Blood Moons Prevented: " + CosmicanonCounts.EclipsesPrevented);
                        CosmicanonCounts.BloodMoonsPrevented = reader.ReadUInt16();
                        l?.Log("Updated Blood Moons Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }
                    break;

                case PacketType.PreventedGlimmer:
                    {
                        l?.Log("Old Glimmers Prevented: " + CosmicanonCounts.EclipsesPrevented);

                        CosmicanonCounts.GlimmersPrevented = reader.ReadUInt16();

                        l?.Log("Updated Glimmers Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }
                    break;

                case PacketType.PreventedEclipse:
                    {
                        l?.Log("Old Eclipses Prevented: " + CosmicanonCounts.EclipsesPrevented);

                        CosmicanonCounts.EclipsesPrevented = reader.ReadUInt16();

                        l?.Log("Updated Eclipses Prevented: " + CosmicanonCounts.EclipsesPrevented);
                    }
                    break;

                case PacketType.UpdateWindSpeeds:
                    {
                        l?.Log("Old Wind variables: {Speed:" + Main.windSpeedSet + ", SpeedSpeed" + Main.windSpeedSpeed + ", TemporarySpeed:" + Main.windSpeedTemp + "}");

                        Main.windSpeedSet = reader.ReadSingle();
                        Main.windSpeedSpeed = reader.ReadSingle();
                        Main.windSpeedTemp = reader.ReadSingle();

                        l?.Log("Updated Wind variables: {Speed:" + Main.windSpeedSet + ", SpeedSpeed" + Main.windSpeedSpeed + ", TemporarySpeed:" + Main.windSpeedTemp + "}");
                    }
                    break;

                case PacketType.RequestOmegaStarite:
                    {
                        if (Main.netMode == NetmodeID.Server) // do not run on multiplayer clients!
                        {
                            AQMod.spawnStarite = true;
                            l?.Log("Setting AQMod.spawnStarite to true!");
                        }
                    }
                    break;

                case PacketType.ActivateGlimmerEvent:
                    {
                        GlimmerEvent.tileX = reader.ReadUInt16();
                        GlimmerEvent.tileY = reader.ReadUInt16();
                        l?.Log("Activated Glimmer Event at: {x:" + GlimmerEvent.tileX + ", y:" + GlimmerEvent.tileY + "}!");
                    }
                    break;

                case PacketType.RequestDemonSiege:
                    {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        int player = reader.ReadInt32();
                        int itemType = reader.ReadInt32();
                        int itemStack = reader.ReadInt32();
                        int itemPrefix = reader.ReadByte();

                        Item item = new Item();

                        item.netDefaults(itemType);
                        item.stack = itemStack;
                        item.Prefix(itemPrefix);

                        if (itemType > Main.maxItemTypes)
                        {
                            item.modItem.NetRecieve(reader);
                        }

                        if (AQMod.Debug.LogNetcode)
                        {
                            l.Value.Log("x: " + x);
                            l.Value.Log("y: " + y);
                            l.Value.Log("player activator: " + player + " (" + Main.player[player].name + ")");
                            l.Value.Log("item Type: " + itemType + " (" + Lang.GetItemName(item.type) + ")");
                            l.Value.Log("item Stack: " + itemStack);
                            l.Value.Log("item Prefix: " + itemPrefix);
                        }

                        DemonSiege.Activate(x, y, player, item, fromServer: true);
                    }
                    break;

                case PacketType.Flag_ExporterIntroduction:
                    {
                    }
                    break;

                case PacketType.Flag_PhysicistIntroduction:
                    {
                        WorldDefeats.PhysicistIntroduction = true;
                    }
                    break;

                case PacketType.Flag_AirHunterIntroduction:
                    {
                        WorldDefeats.HunterIntroduction = true;
                    }
                    break;

                    //case AQPacketID.UpdateAQPlayerCelesteTorus:
                    //    {
                    //        var player = Main.player[reader.ReadByte()];
                    //        var aQPlayer = player.GetModPlayer<AQPlayer>();
                    //        aQPlayer.celesteTorusX = reader.ReadSingle();
                    //        aQPlayer.celesteTorusY = reader.ReadSingle();
                    //        aQPlayer.celesteTorusZ = reader.ReadSingle();

                    //        if (Debug.LogNetcode)
                    //        {
                    //            var l = Debug.GetDebugLogger();
                    //            l.Log("Updating celeste torus positions for: (" + player.name + ")");
                    //            l.Log("x: " + aQPlayer.celesteTorusX);
                    //            l.Log("y: " + aQPlayer.celesteTorusY);
                    //            l.Log("z: " + aQPlayer.celesteTorusZ);
                    //        }
                    //    }
                    //    break;

                    //case AQPacketID.UpdateAQPlayerEncoreKills:
                    //    {
                    //        var player = Main.player[reader.ReadByte()];
                    //        var aQPlayer = player.GetModPlayer<AQPlayer>();
                    //        byte[] buffer = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
                    //        aQPlayer.DeserialzeBossKills(buffer);
                    //    }
                    //    break;
                    //    break;
            }
        }
    }
}