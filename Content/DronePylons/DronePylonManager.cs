using Aequus.Projectiles.Misc.Drones;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.DronePylons
{
    public class DronePylonManager : TagSerializable
    {
        public static int DefaultMaxDrones;

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

        public int maxDrones;
        public List<DroneType> ActiveDrones { get; private set; }

        public bool isActive;

        public DronePylonManager()
        {
            ActiveDrones = new List<DroneType>();
            maxDrones = DefaultMaxDrones;
            location = Point.Zero;
            isActive = true;
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
            if (maxDrones != DefaultMaxDrones)
                tag["MaxDrones"] = maxDrones;
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
                    d.maxDrones = maxDrones;
                }
                if (tag.TryGet<List<TagCompound>>("Drones", out var droneSaves))
                {
                    foreach (var drone in droneSaves)
                    {
                        if (DroneType.KeyToDroneType.TryGetValue(drone.Get<string>("Mod") + "/" + drone.Get<string>("Name"), out var droneType))
                        {
                            d.ActiveDrones.Add((DroneType)droneType.Clone());
                        }
                    }
                }
                return d;
            }
            return null;
        }

        public virtual void SoftUpdate()
        {

        }

        public virtual void HardUpdate()
        {
            if (isActive)
            {
                if (ActiveDrones.Count > 0)
                {
                    var quickList = new List<Point>();
                    foreach (var d in ActiveDrones)
                    {
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
                            Projectile.NewProjectile(null, spawnLocation + Main.rand.NextVector2Unit() * Main.rand.NextFloat(100f), Main.rand.NextVector2Unit() * 5f, p.X, 20, 1f, Main.myPlayer);
                    }
                }
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
            if (ActiveDrones.Count >= maxDrones)
                return false;
            ActiveDrones.Add((DroneType)ModContent.GetInstance<T>().Clone());
            return true;
        }
    }
}