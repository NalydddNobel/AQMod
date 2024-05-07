using tModLoaderExtended.Networking;
using System.IO;

namespace Aequus.Old.Content.Necromancy.Networking;

public class ZombieConvertEffectsPacket : PacketHandler {
    public void Send(NPC npc, int zombieOwner, int renderLayer, int ignoreClient = -1) {
        SendInner(npc.position, npc.width, npc.height, zombieOwner, renderLayer, ignoreClient);
    }

    private void SendInner(Vector2 position, int width, int height, int zombieOwner, int renderLayer, int ignoreClient = -1) {
        ModPacket p = GetPacket();
        p.Write(position.X);
        p.Write(position.Y);
        p.Write(width);
        p.Write(height);
        p.Write(zombieOwner);
        p.Write(renderLayer);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        float X = reader.ReadSingle();
        float Y = reader.ReadSingle();
        int width = reader.ReadInt32();
        int height = reader.ReadInt32();
        int owner = reader.ReadInt32();
        int layer = reader.ReadInt32();

        if (Main.netMode == NetmodeID.Server) {
            SendInner(new Vector2(X, Y), width, height, owner, layer, sender);
        }
        else {
            NecromancyNPC.ConvertEffects(new Vector2(X, Y), width, height, owner, layer);
        }
    }
}
