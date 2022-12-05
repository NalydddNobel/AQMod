using Aequus.NPCs.Friendly.Town.Drones;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.Content.DronePylons
{
    public abstract class DroneSlot : ModType, TagSerializable
    {
        public abstract int NPCType { get; }
        public virtual int SpawnDronesAmount => 1;

        public Point Location { get; internal set; }
        public Vector2 WorldLocation
        {
            get
            {
                return Location.ToWorldCoordinates() + new Vector2(24f);
            }
        }

        internal static int NextNetID;
        public int NetID { get; private set; }

        public static Dictionary<string, DroneSlot> KeyToDroneType { get; private set; }
        public static Dictionary<int, DroneSlot> NetIDToDroneType { get; private set; }

        protected sealed override void Register()
        {
            ModTypeLookup<DroneSlot>.Register(this);
            NetID = NextNetID;
            NextNetID++;
            if (KeyToDroneType == null)
            {
                KeyToDroneType = new Dictionary<string, DroneSlot>();
            }
            if (NetIDToDroneType == null)
            {
                NetIDToDroneType = new Dictionary<int, DroneSlot>();
            }
            KeyToDroneType.Add(FullName, this);
            NetIDToDroneType.Add(NetID, this);
        }

        public virtual TagCompound SerializeData()
        {
            return new TagCompound()
            {
                ["Mod"] = Mod.Name,
                ["Name"] = Name,
            };
        }

        public static List<TagCompound> SerializeData(List<DroneSlot> drones)
        {
            if (drones == null || drones.Count == 0)
            {
                return null;
            }
            var l = new List<TagCompound>();
            foreach (var d in drones)
            {
                l.Add(d.SerializeData());
            }
            return l;
        }

        public virtual void DeserializeData(TagCompound tag)
        {
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void OnAdd(Player player)
        {
        }

        public virtual void OnRemove(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCType && Main.npc[i].ModNPC is TownDroneBase townDrone)
                {
                    if (townDrone.pylonSpot == Location)
                    {
                        Main.npc[i].localAI[0] = 0f;
                        Main.npc[i].Kill();
                        break;
                    }
                }
            }
        }

        public virtual void OnSoftUpdate()
        {

        }

        public virtual void OnHardUpdate()
        {
        }

        public virtual void SendData(BinaryWriter packet)
        {
        }

        public virtual void ReceiveData(BinaryReader reader)
        {
        }

        public PylonDronePoint GetDronePoint()
        {
            return DroneWorld.FindOrAddDrone(Location);
        }
    }
}