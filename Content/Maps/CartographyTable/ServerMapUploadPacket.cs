using Aequus.Common.Net;
using System.IO;
using Terraria.Map;

namespace Aequus.Content.Maps.CartographyTable;

public class ServerMapUploadPacket : PacketHandler {
    public override PacketType LegacyPacketType => PacketType.CartographyTableSubmit;

    private ushort _nextChunk;

    private int TrySendClientChunk(int player) {
        BinaryWriter p = GetPacket();
        ushort chunk = _nextChunk;
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
                MapTile tile = Main.Map[i, j];
                p.Write(tile.Light);
                any |= tile.Light > 0;
            }
        }

        if (!any) {
            return 0;
        }

        SendPacket(p);

        return 1;
    }

    public void SendReset(int player) {
        if (Main.netMode == NetmodeID.Server) {
            BinaryWriter p = GetPacket();
            p.Write((byte)1);
            SendPacket(p, toClient: player);

            ModContent.GetInstance<CartographyTableSystem>().Map!.CreateDebugMap();
        }
    }

    public void Send(int player) {
        if (Main.netMode != NetmodeID.Server) {
            // Set download bar to max when sending
            if (_nextChunk == 0) {
                ServerMapDownloadUI.Instance.SetDownloadProgress(1f);
            }

            int result;
            while ((result = TrySendClientChunk(player)) == 0) {
                _nextChunk++;
            }

            if (result == 2) {
                ServerMapDownloadUI.Instance.EndNet();
                _nextChunk = 0;
            }
            else {
                // Set cartography table upload progress bar.
                ServerMapDownloadUI.Instance.SetUploadProgress(CartographyTableSystem.Instance.Map!.GetChunkProgress(_nextChunk));

                _nextChunk++;
            }
        }
        else {
            BinaryWriter p = GetPacket();
            p.Write((byte)0);
            SendPacket(p, toClient: player);
        }
    }

    public override void Receive(BinaryReader reader, int sender) {
        if (Main.netMode == NetmodeID.Server) {
            ushort chunk = reader.ReadUInt16();

            ServerMap map = ModContent.GetInstance<CartographyTableSystem>().Map!;

            map.GetChunkDimensions(chunk, out int x, out int y, out int w, out int h);
            for (int i = x; i < x + w; i++) {
                for (int j = y; j < y + h; j++) {
                    byte light = reader.ReadByte();
                    if (light > map[i, j].Light) {
                        map.SetLight(i, j, light);
                    }
                    map.ScanType(i, j);
                }
            }
        }
        else {
            byte note = reader.ReadByte();
            if (note == 1) {
                _nextChunk = 0;
            }
        }

        Send(sender);
    }
}