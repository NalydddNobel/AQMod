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
    public class DronePylonSystem : ModSystem
    {
        public class DronePylonData : TagSerializable
        {
            public static int MaxGunners;
            public static int MaxHealers;
            public static int MaxCleaners;

            internal Point location;
            public Point Location 
            { 
                get => location;

                set
                {
                    if (!Drones.ContainsKey(location))
                    {
                        //throw new Exception("Tried to set location for unlisted pylon data. Add DronePylonData to DronePylonSystem.Pylons before adjusting Location.");
                    }
                    if (Drones.ContainsKey(value))
                    {
                        throw new Exception("There is already pylon drone data at " + value);
                    }
                    Drones.Remove(location);
                    Drones.Add(value, this);
                    location = value;
                }
            }

            public int numGunners;
            public int numHealers;
            public int numCleaners;

            public bool isActive;

            public TagCompound SerializeData()
            {
                return new TagCompound()
                {
                    ["Location"] = Location.ToVector2(),
                    ["Gunners"] = numGunners,
                    ["Healers"] = numHealers,
                    ["Cleaners"] = numCleaners,
                };
            }

            public static DronePylonData DeserializeData(TagCompound tag)
            {
                if (tag.TryGet<Vector2>("Location", out var loc))
                {
                    var d = new DronePylonData();
                    d.location = loc.ToPoint();
                    d.numGunners = tag.Get<int>("Gunners");
                    d.numHealers = tag.Get<int>("Healers");
                    d.numCleaners = tag.Get<int>("Cleaners");
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
                    if (numGunners > 0)
                    {
                        int gunnersOwned = 0;
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<GunnerDrone>())
                            {
                                var gunner = Main.projectile[i].ModProjectile<GunnerDrone>();
                                if (gunner.pylonSpot == Location)
                                {
                                    gunnersOwned++;
                                }
                            }
                        }
                        if (gunnersOwned < numGunners)
                        {
                            var spawnLocation = Location.ToWorldCoordinates() + new Vector2(24f);
                            Projectile.NewProjectile(null, spawnLocation, Main.rand.NextVector2Unit() * 5f, ModContent.ProjectileType<GunnerDrone>(), 20, 1f, Main.myPlayer);
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
        }

        public static Dictionary<Point, DronePylonData> Drones { get; private set; }
        public static int HardUpdateDelay;

        public override void Load()
        {
            Drones = new Dictionary<Point, DronePylonData>();
            DronePylonData.MaxGunners = 2;
            DronePylonData.MaxHealers = 1;
            DronePylonData.MaxCleaners = 1;
        }

        public override void Unload()
        {
            Clear();
            Drones = null;
        }

        public void Clear()
        {
            Drones?.Clear();
        }

        public override void OnWorldLoad()
        {
            Clear();
        }

        public override void OnWorldUnload()
        {
            Clear();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (Drones.Count == 0)
                return;
            var l = new List<TagCompound>();
            foreach (var p in Drones)
            {
                l.Add(p.Value.SerializeData());
            }
            tag["Pylons"] = l;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.TryGet<List<TagCompound>>("Pylons", out var pylons))
            {
                foreach (var t in pylons)
                {
                    //foreach (var pair in t)
                    //{
                    //    Mod.Logger.Info(pair.Key + ": " + pair.Value);
                    //}
                    var p = DronePylonData.DeserializeData(t);
                    if (p != null)
                        Drones.Add(p.Location, p);
                }
            }
        }

        public override void PostUpdatePlayers()
        {
            if (AequusHelpers.debugKey && Main.GameUpdateCount % 120 == 0)
            {
                if (ValidSpot(AequusHelpers.tileX, AequusHelpers.tileY))
                {
                    AddDrone(AequusHelpers.tileX, AequusHelpers.tileY);
                }
            }

            foreach (var d in Drones.Values)
            {
                d.SoftUpdate();
            }
            HardUpdateDelay--;
            if (HardUpdateDelay <= 0)
            {
                HardUpdateDrones();
                HardUpdateDelay = Main.netMode == NetmodeID.SinglePlayer ? Math.Max(70 - Main.frameRate, 10) : 10;
            }
        }

        public static void HardUpdateDrones()
        {
            var remove = new List<Point>();
            var p = new List<Player>();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    p.Add(Main.player[i]);
                }
            }
            foreach (var pair in Drones)
            {
                if (!ValidSpot(pair.Key.X, pair.Key.Y))
                {
                    remove.Add(pair.Key);
                    continue;
                }
                var d = pair.Value;
                d.CheckActive(p);
                d.HardUpdate();
            }
            foreach (var d in remove)
            {
                Drones.Remove(d);
            }
        }

        public static bool TryGetDroneData(int i, int j, out DronePylonData drones)
        {
            drones = null;
            if (!ValidSpot(i, j))
            {
                return false;
            }
            return Drones.TryGetValue(FixedPoint(i, j), out drones);
        }

        public static DronePylonData AddDrone(int i, int j)
        {
            var d = new DronePylonData() { location = FixedPoint(i, j), };
            Drones.Add(d.Location, d);
            return d;
        }

        public static DronePylonData FindOrAddDrone(int i, int j)
        {
            if (TryGetDroneData(i, j, out var data))
            {
                return data;
            }
            return AddDrone(i, j);
        }

        public static Point FixedPoint(int i, int j)
        {
            i -= Main.tile[i, j].TileFrameX % 54 / 18;
            j -= Main.tile[i, j].TileFrameY % 72 / 18;
            return new Point(i, j);
        }

        public static bool ValidSpot(int i, int j)
        {
            return Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.TeleportationPylon;
        }
    }
}