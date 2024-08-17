using Aequus.Common.Net;
using System.IO;

namespace Aequus.Content.Maps.CartographyTable;

public class ServerMapRequestPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.CartographyTable;

    private readonly ushort[] _nextSection = new ushort[Main.maxPlayers];

    private int TrySendServerChunk(int player) {
        ModPacket p = GetPacket();
        ushort chunk = _nextSection[player];
        p.Write(chunk);

        ServerMap map = ModContent.GetInstance<CartographyTableSystem>().Map!;
        map.GetChunkDimensions(chunk, out int x, out int y, out int w, out int h);

        if (w <= 0 || h <= 0) {
            // Reached the end.
            return 2;
        }

        bool any = false;

        for (int i = x; i < x + w; i++) {
            for (int j = y; j < y + h; j++) {
                any |= map[i, j].Write(p);
            }
        }

        if (!any) {
            return 0;
        }

        p.Send(toClient: player);

        return 1;
    }

    public void SendReset(int player) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ModPacket p = GetPacket();
            p.Write((byte)1);
            p.Send();
        }
    }

    public void Send(int player) {
        if (Main.netMode == NetmodeID.Server) {
            int result;
            while ((result = TrySendServerChunk(player)) == 0) {
                _nextSection[player]++;
            }

            if (result == 2) {
                _nextSection[player] = 0;
                ModContent.GetInstance<ServerMapClientSubmitPacket>().SendReset(player);
            }
            else {
                _nextSection[player]++;
            }
        }
        else {
            ModPacket p = GetPacket();
            p.Write((byte)0);
            p.Send();
        }
    }

    public override void Receive(BinaryReader reader, int sender) {
        if (Main.netMode == NetmodeID.MultiplayerClient) {
            ushort chunk = reader.ReadUInt16();

            ServerMap map = ModContent.GetInstance<CartographyTableSystem>().Map!;
            map.GetChunkDimensions(chunk, out int x, out int y, out int w, out int h);
            for (int i = x; i < x + w; i++) {
                for (int j = y; j < y + h; j++) {
                    map[i, j] = ServerMapTile.FromReader(reader);
                    map[i, j].ApplyToClient(i, j);
                }
            }

            Main.NewText($"Getting chunk: {chunk}");
        }

        if (Main.netMode == NetmodeID.Server) {
            if (sender < 0 || sender >= Main.maxPlayers) {
                return;
            }

            byte note = reader.ReadByte();
            if (note == 1) {
                _nextSection[sender] = 0;
            }
        }

        Send(sender);
    }
}
