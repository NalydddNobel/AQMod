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
    public class DroneSystem : ModSystem
    {
        public static Dictionary<Point, DronePylonManager> Drones { get; private set; }
        public static int HardUpdateDelay;

        public override void Load()
        {
            Drones = new Dictionary<Point, DronePylonManager>();
            DronePylonManager.DefaultMaxDrones = 2;
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
                    foreach (var pair in t)
                    {
                        Mod.Logger.Info(pair.Key + ": " + pair.Value);
                    }
                    var p = DronePylonManager.DeserializeData(t);
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
                if (d.isActive || Main.rand.NextBool(Math.Clamp(60 - Main.frameRate, 5, 25)))
                    d.HardUpdate();
            }
            foreach (var d in remove)
            {
                Drones.Remove(d);
            }
        }

        public static bool TryGetDroneData(int i, int j, out DronePylonManager drones)
        {
            drones = null;
            if (!ValidSpot(i, j))
            {
                return false;
            }
            return Drones.TryGetValue(FixedPoint(i, j), out drones);
        }

        public static DronePylonManager AddDrone(int i, int j)
        {
            var d = new DronePylonManager() { location = FixedPoint(i, j), };
            Drones.Add(d.Location, d);
            return d;
        }

        public static DronePylonManager FindOrAddDrone(int i, int j)
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