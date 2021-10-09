using AQMod.Items.Tools.Markers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Tiles
{
    public class TEGlobe : ModTileEntity
    {
        public List<MapMarker> markers;
        public bool discovered;

        public TEGlobe()
        {
            markers = new List<MapMarker>();
            discovered = true;
        }

        public TEGlobe(bool discovered)
        {
            markers = new List<MapMarker>();
            this.discovered = discovered;
        }

        public void Discover()
        {
            if (!discovered)
            {
                discovered = true;
                //AQMod.BroadcastMessage("Mods.AQMod.Common.FoundGlobe", new Color(255, 255, 200, 255));
            }
        }

        public bool AlreadyHasMarker(string mod, string name)
        {
            return markers.Find((m) => m.mod.Name == mod && m.Name == name) != null;
        }

        public bool AlreadyHasMarker(MapMarker marker)
        {
            return markers.Find((m) => m.mod.Name == marker.mod.Name && m.Name == marker.Name) != null;
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(discovered);
            writer.Write(markers.Count);
            foreach (var marker in markers)
            {
                writer.Write(marker.mod.Name);
                writer.Write(marker.Name);
            }
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            discovered = reader.ReadBoolean();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string mod = reader.ReadString();
                string name = reader.ReadString();
                AddMarker(mod, name);
            }
        }

        public bool AddMarker(string mod, string name)
        {
            var m = Globe._registeredMarkers.Find((marker) => marker.mod.Name == mod && marker.Name == name);
            if (m == null)
            {
                return false;
            }
            AddMarker(m);
            return true;
        }

        public void AddMarker(MapMarker marker)
        {
            markers.Add(marker);
        }

        public override TagCompound Save()
        {
            var tag = new TagCompound
            {
                ["discovered"] = discovered,
                ["MarkerCount"] = markers.Count,
            };
            for (int i = 0; i < markers.Count; i++)
            {
                MapMarker marker = markers[i];
                tag["markermod" + i] = marker.mod.Name;
                tag["markername" + i] = marker.Name;
            }
            return tag;
        }

        public override void Load(TagCompound tag)
        {
            discovered = tag.GetBool("discovered");
            int count = tag.GetInt("MarkerCount");
            for (int i = 0; i < count; i++)
            {
                AddMarker(tag.GetString("markermod" + i), tag.GetString("markername" + i));
            }
        }

        public override bool ValidTile(int i, int j)
        {
            var t = Framing.GetTileSafely(i, j);
            return t.active() && t.type == ModContent.TileType<Globe>() && t.frameX == 0 && t.frameY == 0;
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 2);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }

        public override void OnKill()
        {
            foreach (var m in markers)
            {
                Item.NewItem(new Rectangle(Position.X * 16, Position.Y * 16, 32, 32), m.item.type);
            }
        }
    }
}