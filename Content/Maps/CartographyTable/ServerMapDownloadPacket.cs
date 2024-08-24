using Aequus.Common.Net;
using System.IO;

namespace Aequus.Content.Maps.CartographyTable;

public class ServerMapDownloadPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.CartographyTable;

    private readonly ushort[] _nextSection = new ushort[Main.maxPlayers];

    private int TrySendServerChunk(int player) {
        BinaryWriter p = GetPacket();
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

        SendPacket(p, toClient: player);

        return 1;
    }

    public void SendReset(int player) {
        if (Main.netMode != NetmodeID.Server) {
            BinaryWriter p = GetPacket();
            p.Write((byte)1);
            SendPacket(p);

            CartographyTableSystem.Instance.Map!.ResetNoUploadList();
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
                ModContent.GetInstance<ServerMapUploadPacket>().SendReset(player);
            }
            else {
                _nextSection[player]++;
            }
        }
        else {
            BinaryWriter p = GetPacket();
            p.Write((byte)0);
            SendPacket(p);
        }
    }

    public override void Receive(BinaryReader reader, int sender) {
        if (Main.netMode != NetmodeID.Server) {
            // Read the chunk Id.
            ushort chunk = reader.ReadUInt16();

            ServerMap map = ModContent.GetInstance<CartographyTableSystem>().Map!;

            map.GetChunkDimensions(chunk, out int x, out int y, out int w, out int h);
            map.SetNoUpload(chunk, true);
            for (int i = x; i < x + w; i++) {
                for (int j = y; j < y + h; j++) {
                    map[i, j] = ServerMapTile.FromReader(reader);
                    if (map[i, j].ApplyToClient(i, j)) {
                        map.SetNoUpload(chunk, false);
                    }
                }
            }

            // Set cartography table download progress bar.
            ServerMapDownloadUI.Instance.SetDownloadProgress(map.GetChunkProgress(chunk));
        }
        else {
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
