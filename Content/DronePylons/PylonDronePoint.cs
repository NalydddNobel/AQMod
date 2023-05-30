using Aequus.Content.DronePylons.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.DronePylons {
    public class PylonDronePoint : TagSerializable
    {
        public static int DefaultMaxDrones;

        public bool isActive;

        public int MaxDrones;

        public int hardUpdates;
        public int netUpdates;
        public int netUpdateSkip;

        internal Point location;
        public Point Location
        {
            get => location;

            set
            {
                //if (!DroneWorld.Drones.ContainsKey(location))
                //{
                //    throw new Exception("Tried to set location for unlisted pylon data. Add DronePylonData to DronePylonSystem.Pylons before adjusting Location.");
                //}
                if (DroneWorld.Drones.ContainsKey(value))
                {
                    throw new Exception("There is already pylon drone data at " + value);
                }
                DroneWorld.Drones.Remove(location);
                DroneWorld.Drones.Add(value, this);
                location = value;
            }
        }
        public Vector2 WorldLocation
        {
            get
            {
                return Location.ToWorldCoordinates() + new Vector2(24f);
            }
        }

        public List<DroneSlot> ActiveDrones { get; private set; }

        public int Count
        {
            get
            {
                int numOwned = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].ModNPC is TownDroneBase townDrone)
                    {
                        if (townDrone.pylonSpot == Location)
                        {
                            numOwned++;
                        }
                    }
                }
                return numOwned;
            }
        }
        public List<NPC> OwnedDrones
        {
            get
            {
                var l = new List<NPC>();
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].ModNPC is TownDroneBase townDrone)
                    {
                        if (townDrone.pylonSpot == Location)
                        {
                            l.Add(Main.npc[i]);
                        }
                    }
                }
                return l;
            }
        }
        public List<NPC> NearbyTownNPCs
        {
            get
            {
                var l = new List<NPC>();
                var worldLocation = WorldLocation;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].townNPC && !Main.npc[i].homeless)
                    {
                        float d = Vector2.Distance(worldLocation, Main.npc[i].Home().ToWorldCoordinates());
                        if (d < 3000f)
                        {
                            l.Add(Main.npc[i]);
                        }
                    }
                }
                return l;
            }
        }

        public PylonDronePoint()
        {
            ActiveDrones = new List<DroneSlot>();
            MaxDrones = DefaultMaxDrones;
            location = Point.Zero;
            isActive = true;
            hardUpdates = 1;
        }

        public TagCompound SerializeData()
        {
            var tag = new TagCompound()
            {
                ["Location"] = Location.ToVector2(),
            };
            var droneSaveData = DroneSlot.SerializeData(ActiveDrones);
            if (droneSaveData != null)
                tag["Drones"] = droneSaveData;
            if (MaxDrones != DefaultMaxDrones)
                tag["MaxDrones"] = MaxDrones;
            return tag;
        }

        public static PylonDronePoint DeserializeData(TagCompound tag)
        {
            if (tag.TryGet<Vector2>("Location", out var loc))
            {
                var d = new PylonDronePoint
                {
                    location = loc.ToPoint()
                };
                if (tag.TryGet("maxDrones", out int maxDrones))
                {
                    d.MaxDrones = maxDrones;
                }
                if (tag.TryGet<List<TagCompound>>("Drones", out var droneSaves))
                {
                    foreach (var droneTag in droneSaves)
                    {
                        if (DroneSlot.KeyToDroneType.TryGetValue(droneTag.Get<string>("Mod") + "/" + droneTag.Get<string>("Name"), out var droneType))
                        {
                            var droneInstance = (DroneSlot)droneType.Clone();
                            droneInstance.Location = d.Location;
                            droneInstance.DeserializeData(droneTag);
                            d.ActiveDrones.Add(droneInstance);
                        }
                    }
                }
                return d;
            }
            return null;
        }

        public virtual void SoftUpdate()
        {
            if (isActive)
            {
                foreach (var d in ActiveDrones)
                {
                    d.OnSoftUpdate();
                }
            }
        }

        public virtual void HardUpdate()
        {
            if (isActive || Main.netMode != NetmodeID.SinglePlayer || Main.rand.NextBool(Math.Clamp(60 - Main.frameRate, 5, 25)))
            {
                if (netUpdates > 50)
                {
                    netUpdates = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        Sync();
                    }
                }
                if (ActiveDrones.Count > 0)
                {
                    var quickList = new List<Point>();
                    foreach (var d in ActiveDrones)
                    {
                        d.Location = Location;
                        d.OnHardUpdate();
                        int proj = d.NPCType;
                        for (int i = 0; i < quickList.Count; i++)
                        {
                            if (quickList[i].X == proj)
                            {
                                quickList[i] = new Point(proj, quickList[i].Y + d.SpawnDronesAmount);
                                goto Continue;
                            }
                        }
                        quickList.Add(new Point(proj, d.SpawnDronesAmount));
                    Continue:
                        continue;
                    }

                    if (isActive)
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active && Main.npc[i].friendly && Main.npc[i].ModNPC is TownDroneBase townDrone)
                            {
                                if (townDrone.pylonSpot == Location)
                                {
                                    for (int k = 0; k < quickList.Count; k++)
                                    {
                                        if (Main.npc[i].type == quickList[k].X)
                                        {
                                            if (quickList[k].Y <= 1)
                                            {
                                                quickList.RemoveAt(k);
                                                break;
                                            }
                                            quickList[k] = new Point(quickList[k].X, quickList[k].Y - 1);
                                        }
                                    }
                                }
                            }
                        }

                        if (quickList.Count > 0)
                        {
                            var spawnLocation = Location.ToWorldCoordinates() + new Vector2(24f);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                foreach (var p in quickList)
                                {
                                    for (int i = 0; i < p.Y; i++)
                                        NPC.NewNPCDirect(new EntitySource_SpawnNPC("Pylon"), spawnLocation + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f), p.X);
                                }
                            }
                        }
                    }
                }
                hardUpdates = 1;
                if (netUpdateSkip > 5 || Main.rand.NextBool())
                {
                    netUpdates++;
                    netUpdateSkip = 0;
                }
                else
                {
                    netUpdateSkip++;
                }
            }
            else
            {
                hardUpdates++;
            }
        }

        public void CheckActive(List<Player> snippedPlayerList)
        {
            if (IsActive(snippedPlayerList))
            {
                if (!isActive)
                {
                    isActive = true;
                    Activate();
                }
                return;
            }
            if (isActive)
            {
                isActive = false;
                Deactivate();
            }
        }

        public virtual bool IsActive(List<Player> snippedPlayerList)
        {
            var worldPos = Location.ToWorldCoordinates() + new Vector2(24f, 0f);
            foreach (var p in snippedPlayerList)
            {
                if (Vector2.Distance(p.Center, worldPos) < 4000f)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void Activate()
        {
        }

        public virtual void Deactivate()
        {
        }

        public bool ConsumeSlot<T>(Player player = null) where T : DroneSlot
        {
            if (ActiveDrones.Count >= MaxDrones)
            {
                for (int i = 0; i < ActiveDrones.Count; i++)
                {
                    if (ActiveDrones[i] is T)
                    {
                        continue;
                    }
                    ActiveDrones[i].OnRemove(player);
                    ActiveDrones[i] = (DroneSlot)ModContent.GetInstance<T>().Clone();
                    ActiveDrones[i].OnAdd(player);
                    if (player == null || Main.myPlayer == player.whoAmI)
                        Sync();
                    return true;
                }
                return false;
            }
            var d = (DroneSlot)ModContent.GetInstance<T>().Clone();
            d.Location = Location;
            d.OnAdd(player);
            ActiveDrones.Add(d);
            if (player == null || Main.myPlayer == player.whoAmI)
                Sync();
            return true;
        }

        public void Sync() // Epic fail L
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            PacketSystem.Send((p) =>
            {
                SendData(p);
            }, PacketType.SyncDronePoint);
        }

        public void SendData(BinaryWriter p)
        {
            p.Write(location.X);
            p.Write(location.Y);
            p.Write(MaxDrones);
            p.Write(ActiveDrones.Count);
            foreach (var d in ActiveDrones)
            {
                p.Write(d.NetID);
                d.SendData(p);
            }
        }

        public void RecieveData(BinaryReader reader)
        {
            MaxDrones = reader.ReadInt32();
            int amt = reader.ReadInt32();
            //Aequus.Instance.Logger.Debug($"Has {amt} drones");
            for (int i = 0; i < amt; i++)
            {
                int netID = reader.ReadInt32();
                //Aequus.Instance.Logger.Debug($"NetID: {netID} ({DroneSlot.NetIDToDroneType[netID].FullName})");
                if (ActiveDrones.Count <= i)
                {
                    ActiveDrones.Add((DroneSlot)DroneSlot.NetIDToDroneType[netID].Clone());
                }
                if (ActiveDrones[i].NetID != netID)
                {
                    ActiveDrones[i] = (DroneSlot)DroneSlot.NetIDToDroneType[netID].Clone();
                }

                ActiveDrones[i].Location = location;
                ActiveDrones[i].ReceiveData(reader);
            }
        }
    }
}