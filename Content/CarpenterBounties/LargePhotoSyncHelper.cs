using Microsoft.Xna.Framework;
using System;
using System.IO;

namespace Aequus.Content.CarpenterBounties
{
    public class LargePhotoSyncHelper
    {
        public PhotoInfo photo;
        public byte[] buffer;
        public int bufferProgress;

        public LargePhotoSyncHelper()
        {
            photo = null;
            buffer = null;
            bufferProgress = 0;
        }

        public LargePhotoSyncHelper(PhotoInfo photo)
        {
            this.photo = photo;
            buffer = photo.tileMap.CompressTileArray();
            bufferProgress = 0;
        }

        public bool Update()
        {
            var p = Aequus.GetPacket(PacketType.RegisterPhotoClip);
            p.Write(photo.ID);
            p.Write(photo.time);
            p.Write(photo.world.X);
            p.Write(photo.world.Y);
            p.Write(photo.tileMap.WorldID);
            p.Write(photo.tileMap.Area.X);
            p.Write(photo.tileMap.Area.Y);
            p.Write(photo.tileMap.Area.Width);
            p.Write(photo.tileMap.Area.Height);
            p.Write(buffer.Length);
            p.Write(bufferProgress);
            for (int i = bufferProgress; i < Math.Min(bufferProgress + 100, buffer.Length); i++)
            {
                p.Write(buffer[i]);
            }
            return bufferProgress < buffer.Length;
        }

        public void RecieveInfo(BinaryReader reader)
        {
            ushort time = reader.ReadUInt16();
            var worldFrac = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            int worldID = reader.ReadInt32();
            var worldRect = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            int bufferLength = reader.ReadInt32();
            int bufferProgress = reader.ReadInt32();
        }
    }
}