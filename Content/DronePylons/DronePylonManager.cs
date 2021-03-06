using Aequus.Projectiles.Misc.Drones;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.DronePylons
{
    public class DronePylonManager : TagSerializable
    {
        public static int DefaultMaxDrones;

        public bool isActive;

        public int MaxDrones;

        public int hardUpdates;

        internal Point location;
        public Point Location
        {
            get => location;

            set
            {
                if (!DroneSystem.Drones.ContainsKey(location))
                {
                    //throw new Exception("Tried to set location for unlisted pylon data. Add DronePylonData to DronePylonSystem.Pylons before adjusting Location.");
                }
                if (DroneSystem.Drones.ContainsKey(value))
                {
                    throw new Exception("There is already pylon drone data at " + value);
                }
                DroneSystem.Drones.Remove(location);
                DroneSystem.Drones.Add(value, this);
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
        
        public List<DroneType> ActiveDrones { get; private set; }

        public int Count
        {
            get
            {
                int numOwned = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].ModProjectile is TownDroneBase townDrone)
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
        public List<Projectile> OwnedDrones
        {
            get
            {
                var l = new List<Projectile>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].ModProjectile is TownDroneBase townDrone)
                    {
                        if (townDrone.pylonSpot == Location)
                        {
                            l.Add(Main.projectile[i]);
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
                        if (d < 1000f)
                        {
                            l.Add(Main.npc[i]);
                        }
                    }
                }
                return l;
            }
        }

        public DronePylonManager()
        {
            ActiveDrones = new List<DroneType>();
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
            var droneSaveData = DroneType.SerializeData(ActiveDrones);
            if (droneSaveData != null)
                tag["Drones"] = droneSaveData;
            if (MaxDrones != DefaultMaxDrones)
                tag["MaxDrones"] = MaxDrones;
            return tag;
        }

        public static DronePylonManager DeserializeData(TagCompound tag)
        {
            if (tag.TryGet<Vector2>("Location", out var loc))
            {
                var d = new DronePylonManager();
                d.location = loc.ToPoint();
                if (tag.TryGet("maxDrones", out int maxDrones))
                {
                    d.MaxDrones = maxDrones;
                }
                if (tag.TryGet<List<TagCompound>>("Drones", out var droneSaves))
                {
                    foreach (var droneTag in droneSaves)
                    {
                        if (DroneType.KeyToDroneType.TryGetValue(droneTag.Get<string>("Mod") + "/" + droneTag.Get<string>("Name"), out var droneType))
                        {
                            var droneInstance = (DroneType)droneType.Clone();
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
                if (ActiveDrones.Count > 0)
                {
                    var quickList = new List<Point>();
                    foreach (var d in ActiveDrones)
                    {
                        d.Location = Location;
                        d.OnHardUpdate();
                        int proj = d.ProjectileType;
                        for (int i = 0; i < quickList.Count; i++)
                        {
                            if (quickList[i].X == proj)
                            {
                                quickList[i] = new Point(proj, quickList[i].Y + d.ProjectileAmt);
                                goto Continue;
                            }
                        }
                        quickList.Add(new Point(proj, d.ProjectileAmt));
                    Continue:
                        continue;
                    }

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].friendly && Main.projectile[i].ModProjectile is TownDroneBase townDrone)
                        {
                            var gunner = Main.projectile[i].ModProjectile<TownDroneBase>();
                            if (townDrone.pylonSpot == Location)
                            {
                                for (int k = 0; k < quickList.Count; k++)
                                {
                                    if (Main.projectile[i].type == quickList[k].X)
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

                    if (quickList.Count <= 0)
                        return;

                    var spawnLocation = Location.ToWorldCoordinates() + new Vector2(24f);

                    foreach (var p in quickList)
                    {
                        for (int i = 0; i < p.Y; i++)
                            Projectile.NewProjectile(null, spawnLocation + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10f), Main.rand.NextVector2Unit() * 5f, p.X, 20, 1f, Main.myPlayer);
                    }
                }
                hardUpdates = 1;
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

        public bool AddDrone<T>() where T : DroneType
        {
            if (ActiveDrones.Count >= MaxDrones)
            {
                for (int i = 0; i < ActiveDrones.Count; i++)
                {
                    if (ActiveDrones[i] is T)
                    {
                        continue;
                    }
                    ActiveDrones[i].OnRemove();
                    ActiveDrones[i] = (DroneType)ModContent.GetInstance<T>().Clone();
                    return true;
                }
                return false;
            }
            var d = (DroneType)ModContent.GetInstance<T>().Clone();
            d.Location = Location;
            d.OnAdd();
            ActiveDrones.Add(d);
            return true;
        }
    }
}