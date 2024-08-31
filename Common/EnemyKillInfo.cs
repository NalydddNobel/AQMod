using System.IO;

namespace Aequus.Common.Structures;

public struct EnemyKillInfo(NPC npc) {
    public Vector2 position = npc.position;
    public int width = npc.width;
    public int height = npc.height;
    public int netID = npc.netID;
    public int lifeMax = npc.lifeMax;
    public float value = npc.value;

    public Vector2 Center => new Vector2(position.X + width / 2f, position.Y + height / 2f);
    public Rectangle Rect => new Rectangle((int)position.X, (int)position.Y, width, height);

    public void WriteData(BinaryWriter writer) {
        writer.Write(position.X);
        writer.Write(position.Y);
        writer.Write(width);
        writer.Write(height);
        writer.Write(netID);
        writer.Write(lifeMax);
        writer.Write(value);
    }

    public static EnemyKillInfo ReceiveData(BinaryReader reader) {
        var info = new EnemyKillInfo();
        info.position.X = reader.ReadSingle();
        info.position.Y = reader.ReadSingle();
        info.width = reader.ReadInt32();
        info.height = reader.ReadInt32();
        info.netID = reader.ReadInt32();
        info.lifeMax = reader.ReadInt32();
        info.value = reader.ReadSingle();
        return info;
    }
}