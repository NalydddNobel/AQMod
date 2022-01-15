using AQMod.Tiles.Furniture;
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
        public readonly List<string> LegacyMarkers;

        public TEGlobe()
        {
            LegacyMarkers = new List<string>();
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(LegacyMarkers.Count);
            foreach (var text in LegacyMarkers)
            {
                writer.Write(text);
            }
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                AddMarker(name);
            }
        }

        public bool AddMarker(string name)
        {
            LegacyMarkers.Add(name);
            return true;
        }

        public override TagCompound Save()
        {
            if (LegacyMarkers == null)
                return null;
            var tag = new TagCompound
            {
                ["MarkerCount"] = LegacyMarkers.Count,
                ["SaveType"] = (byte)2,
            };
            for (int i = 0; i < LegacyMarkers.Count; i++)
            {
                tag["markername" + i] = LegacyMarkers[i];
            }
            return tag;
        }


        public override void Load(TagCompound tag)
        {
            if (tag == null) // I don't think it even runs Load if the tag is null, whatever, best to be safe anyways!
                return;
            int count = tag.GetInt("MarkerCount");
            byte b = tag.GetByte("SaveType");
            if (b == 0)
            {
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    AddMarker(tag.GetString("markername" + i));
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