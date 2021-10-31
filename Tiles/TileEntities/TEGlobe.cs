using AQMod.Content;
using AQMod.Content.MapMarkers;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace AQMod.Tiles.TileEntities
{
    public class TEGlobe : ModTileEntity
    {
        public readonly List<MapMarkerData> Markers;
        public bool Discovered { get; set; }

        public TEGlobe()
        {
            Markers = new List<MapMarkerData>();
            Discovered = true;
        }

        public TEGlobe(bool discovered)
        {
            Markers = new List<MapMarkerData>();
            Discovered = discovered;
        }

        public void Discover()
        {
            if (!Discovered)
                Discovered = true;
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(Discovered);
            writer.Write(Markers.Count);
            foreach (var marker in Markers)
            {
                writer.Write(marker.Name);
            }
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            Discovered = reader.ReadBoolean();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                AddMarker(name);
            }
        }

        public bool AddMarker(string name)
        {
            if (AQMod.MapMarkers.TryGetMarker(name, out MapMarkerData value))
                return false;
            Markers.Add(value);
            return true;
        }

        public override TagCompound Save()
        {
            var tag = new TagCompound
            {
                ["discovered"] = Discovered,
                ["MarkerCount"] = Markers.Count,
                ["SaveType"] = (byte)1,
            };
            for (int i = 0; i < Markers.Count; i++)
            {
                var marker = Markers[i];
                tag["markername" + i] = marker.Name;
            }
            return tag;
        }

        private void loadLegacy(TagCompound tag, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (tag.GetString("markermod" + i) == "AQMod")
                {
                    switch (tag.GetString("markername" + i))
                    {
                        case "CosmicTelescope":
                        {
                            AQMod.MapMarkers.GetMarker("CosmicMarker");
                        }
                        break;

                        case "DungeonMap":
                        {
                            AQMod.MapMarkers.GetMarker("DungeonMarker");
                        }
                        break;

                        case "LihzahrdMap":
                        {
                            AQMod.MapMarkers.GetMarker("LihzahrdMarker");
                        }
                        break;

                        case "RetroGoggles":
                        {
                            AQMod.MapMarkers.GetMarker("RetroMarker");
                        }
                        break;
                    }
                }
            }
        }

        public override void Load(TagCompound tag)
        {
            Discovered = tag.GetBool("discovered");
            int count = tag.GetInt("MarkerCount");
            byte b = tag.GetByte("SaveType");
            if (b == 0)
            {
                loadLegacy(tag, count);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    string name = tag.GetString("markername" + i);
                    AQMod.MapMarkers.TryGetMarker(name, out MapMarkerData value);
                }
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
    }
}