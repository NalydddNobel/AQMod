using Microsoft.Xna.Framework;
using System.IO;
using Terraria;

namespace Aequus.Common
{
    public struct EnemyKillInfo
    {
        public Vector2 position;
        public int width;
        public int height;
        public int netID;
        public int lifeMax;
        public float value;
        public BitsByte miscInfo;

        public Vector2 Center => new Vector2(position.X + width / 2f, position.Y + height / 2f);
        public Rectangle Rect => new Rectangle((int)position.X, (int)position.Y, width, height);

        public EnemyKillInfo(NPC npc)
        {
            position = npc.position;
            width = npc.width;
            height = npc.height;
            netID = npc.netID;
            lifeMax = npc.lifeMax;
            value = npc.value;
            miscInfo = 0;
        }

        public void WriteData(BinaryWriter writer)
        {
            writer.Write(position.X);
            writer.Write(position.Y);
            writer.Write(width);
            writer.Write(height);
            writer.Write(netID);
            writer.Write(lifeMax);
            writer.Write(value);
            //writer.Write(miscInfo);
        }

        public static EnemyKillInfo ReceiveData(BinaryReader reader)
        {
            var info = new EnemyKillInfo();
            info.position.X = reader.ReadSingle();
            info.position.Y = reader.ReadSingle();
            info.width = reader.ReadInt32();
            info.height = reader.ReadInt32();
            info.netID = reader.ReadInt32();
            info.lifeMax = reader.ReadInt32();
            info.value = reader.ReadSingle();
            //info.miscInfo = reader.ReadByte();
            return info;
        }
    }
}