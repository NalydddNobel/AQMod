using AQMod.Common;
using AQMod.Common.Utilities.Debugging;
using AQMod.Content;
using AQMod.Content.Players;
using AQMod.Content.World.Events;
using AQMod.NPCs.Friendly;
using Microsoft.Xna.Framework;
using System;
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
            public const ushort CombatText = 4;
            public const ushort CombatNumber = 5;
            public const ushort HealPlayer = 6;

            public const ushort ActivateGlimmerEvent = 500;
            public const ushort RequestOmegaStarite = 1000;
            public const ushort RequestDemonSiege = 2000;
            public const ushort RequestDemonSiegeEnemy = 2001;
            public const ushort RequestDungeonCoordinates = 2002;
            public const ushort RecieveDungeonCoordinates = 2003;
            public const ushort RequestExporterQuestRandomize = 2004;

            public const ushort Flag_ExporterIntroduction = 10000;
            public const ushort Flag_PhysicistIntroduction = 10001;
            public const ushort Flag_AirHunterIntroduction = 10002;
            public const ushort Flag_AirMerchantHasBeenFound = 10003;

            public const ushort Player_SyncEncoreData = 20000;
        }

        public static void WorldStatus()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData);
            }
        }

        #region Demon Siege
        public static void RequestDemonSiege(int x, int y, int plr, Item item)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.GetInstance().GetPacket();
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
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.RequestDemonSiege);
            p.Write(x);
            p.Write(y);
            p.Send();
        }
        #endregion

        #region Glimmer Event 
        public static void RequestOmegaStarite()
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.RequestOmegaStarite);
            p.Send();
        }

        public static void ActivateGlimmerEvent()
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.ActivateGlimmerEvent);
            p.Write(Glimmer.tileX);
            p.Write(Glimmer.tileY);
            p.Send();
        }
        #endregion

        #region Misc
        public static void RequestDungeonCoordinatesUpdate(bool setMapCoords = true)
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.RequestDungeonCoordinates);
            p.Write(setMapCoords);
            p.Write(Main.myPlayer);
            p.Send();
        }

        public static void NetCombatText(Rectangle rect, Color color, int amount, bool dramatic = false, bool dot = false)
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.CombatNumber);
            p.Write(true);
            p.Write(rect.X);
            p.Write(rect.Y);
            p.Write(rect.Width);
            p.Write(rect.Height);
            if (color == default(Color))
            {
                color = new Color(255, 255, 255, 255);
            }
            p.Write(color.R);
            p.Write(color.G);
            p.Write(color.B);
            p.Write(amount);
            p.Send();
        }
        public static void NetCombatText(Vector2 position, Color color, int amount)
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.CombatNumber);
            p.Write(false);
            p.Write(position.X);
            p.Write(position.Y);
            if (color == default(Color))
            {
                color = new Color(255, 255, 255, 255);
            }
            p.Write(color.R);
            p.Write(color.G);
            p.Write(color.B);
            p.Write(amount);
            p.Send();
        }

        public static void NetCombatText(Rectangle rect, Color color, string text)
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.CombatText);
            p.Write(true);
            p.Write(rect.X);
            p.Write(rect.Y);
            p.Write(rect.Width);
            p.Write(rect.Height);
            if (color == default(Color))
            {
                color = new Color(255, 255, 255, 255);
            }
            p.Write(color.R);
            p.Write(color.G);
            p.Write(color.B);
            p.Write(text);
            p.Send();
        }
        public static void NetCombatText(Vector2 position, Color color, string text)
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.CombatText);
            p.Write(false);
            p.Write(position.X);
            p.Write(position.Y);
            if (color == default(Color))
            {
                color = new Color(255, 255, 255, 255);
            }
            p.Write(color.R);
            p.Write(color.G);
            p.Write(color.B);
            p.Write(text);
            p.Send();
        }

        public static void UpdateWindSpeeds()
        {
            var p = AQMod.GetInstance().GetPacket();
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
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.PreventedBloodMoon);
            p.Write(CosmicanonWorldData.BloodMoonsPrevented);
            p.Send();
        }

        public static void PreventedGlimmer()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.PreventedGlimmer);
            p.Write(CosmicanonWorldData.GlimmersPrevented);
            p.Send();
        }

        public static void PreventedEclipse()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            var p = AQMod.GetInstance().GetPacket();
            p.Write(PacketType.PreventedEclipse);
            p.Write(CosmicanonWorldData.EclipsesPrevented);
            p.Send();
        }
        #endregion

        public static void Request(ushort type)
        {
            var p = AQMod.GetInstance().GetPacket();
            p.Write(type);
            p.Send();
        }

        public static void ReadPacket(BinaryReader reader, int sender)
        {
            ushort messageID = reader.ReadUInt16();

            DebugUtilities.Logger? l = null;
            if (DebugUtilities.LogNetcode)
            {
                l = DebugUtilities.GetDebugLogger();
                l.Value.Log("Message ID: " + messageID);
            }

            switch (messageID)
            {
                case PacketType.HealPlayer:
                    {
                        byte plr = reader.ReadByte();
                        ushort healingAmt = reader.ReadUInt16();
                        Main.player[plr].statLife += healingAmt;
                        Main.player[plr].statLife = Math.Min(Main.player[plr].statLife, Main.player[plr].statLifeMax2);
                    }
                    break;

                case PacketType.RequestExporterQuestRandomize:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        Robster.StartRandomHunt();

                        if (Robster.ActiveQuest == null)
                        {
                            l?.Log("no quest was activated");
                        }
                        else
                        {
                            l?.Log("Quest Activated: {" + Robster.ActiveQuest.Key + "}");
                            l?.Log("Type: " + Robster.ActiveQuest.type);
                            l?.Log("Location: " + Robster.ActiveQuest.location);
                            l?.Log("NPC Index: " + Robster.TargetNPC);
                            if (Robster.TargetNPC > -1)
                                l?.Log("NPC Name: " + Main.npc[Robster.TargetNPC].FullName);
                        }

                        NetMessage.SendData(MessageID.WorldData);
                    }
                    break;

                case PacketType.RecieveDungeonCoordinates:
                    if (Main.netMode != NetmodeID.Server)
                    {
                        bool setMapCoords = reader.ReadBoolean();

                        if (setMapCoords)
                        {
                            l?.Log("Old Dungeon Map Coords: {x:" + MapUI.dungeonX + ", y:" + MapUI.dungeonY + "}");

                            MapUI.dungeonX = reader.ReadInt32();
                            MapUI.dungeonY = reader.ReadInt32();

                            l?.Log("New Dungeon Coords: {x:" + MapUI.dungeonX + ", y:" + MapUI.dungeonY + "}");
                        }
                        else
                        {
                            l?.Log("Old Dungeon Coords: {x:" + Main.dungeonX + ", y:" + Main.dungeonY + "}");

                            Main.dungeonX = reader.ReadInt32();
                            Main.dungeonY = reader.ReadInt32();

                            l?.Log("New Dungeon Coords: {x:" + Main.dungeonX + ", y:" + Main.dungeonY + "}");
                        }
                    }
                    break;

                case PacketType.RequestDungeonCoordinates:
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var p = AQMod.GetInstance().GetPacket();
                        p.Write(PacketType.RecieveDungeonCoordinates);
                        p.Write(reader.ReadBoolean());
                        p.Write(Main.dungeonX);
                        p.Write(Main.dungeonY);
                        p.Send(toClient: reader.ReadInt32());
                    }
                    break;

                case PacketType.CombatText:
                case PacketType.CombatNumber:
                    {
                        Rectangle rect;
                        if (reader.ReadBoolean())
                        {
                            rect = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                        }
                        else
                        {
                            Vector2 position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            rect = new Rectangle((int)position.X, (int)position.Y, 2, 2);
                        }
                        var color = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                        // Reading all of this data so that the server doesn't have a random packet or something.
                        if (messageID == PacketType.CombatNumber)
                        {
                            int amount = reader.ReadInt32();
                            if (Main.netMode != NetmodeID.Server)
                            {
                                int c = CombatText.NewText(rect, color, amount, true);
                            }
                        }
                        else
                        {
                            string text = reader.ReadString();
                            if (Main.netMode != NetmodeID.Server)
                            {
                                int c = CombatText.NewText(rect, color, 0, true);
                                Main.combatText[c].text = text; // TODO: make it properly center the text with the string measurement.
                            }
                        }
                    }
                    break;

                case PacketType.PreventedBloodMoon:
                    {
                        l?.Log("Old Blood Moons Prevented: " + CosmicanonWorldData.EclipsesPrevented);
                        CosmicanonWorldData.BloodMoonsPrevented = reader.ReadUInt16();
                        l?.Log("Updated Blood Moons Prevented: " + CosmicanonWorldData.EclipsesPrevented);
                    }
                    break;

                case PacketType.PreventedGlimmer:
                    {
                        l?.Log("Old Glimmers Prevented: " + CosmicanonWorldData.EclipsesPrevented);

                        CosmicanonWorldData.GlimmersPrevented = reader.ReadUInt16();

                        l?.Log("Updated Glimmers Prevented: " + CosmicanonWorldData.EclipsesPrevented);
                    }
                    break;

                case PacketType.PreventedEclipse:
                    {
                        l?.Log("Old Eclipses Prevented: " + CosmicanonWorldData.EclipsesPrevented);

                        CosmicanonWorldData.EclipsesPrevented = reader.ReadUInt16();

                        l?.Log("Updated Eclipses Prevented: " + CosmicanonWorldData.EclipsesPrevented);
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
                            WorldDefeats.OmegaStariteIntroduction = true;
                            l?.Log("Setting AQMod.spawnStarite to true!");
                        }
                    }
                    break;

                case PacketType.ActivateGlimmerEvent:
                    {
                        Glimmer.tileX = reader.ReadUInt16();
                        Glimmer.tileY = reader.ReadUInt16();
                        l?.Log("Activated Glimmer Event at: {x:" + Glimmer.tileX + ", y:" + Glimmer.tileY + "}!");
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

                        if (DebugUtilities.LogNetcode)
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

                case PacketType.Flag_AirMerchantHasBeenFound:
                    {
                        WorldDefeats.AirMerchantHasBeenFound = true;
                    }
                    break;
            }
        }
    }
}