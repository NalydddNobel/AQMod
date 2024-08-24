using System;
using System.Collections.Generic;
using System.IO;

namespace Aequus.Common.Net;
internal class SingleplayerServer : ModSystem {
    private List<BinaryWriter> _nextPackets = [];
    private List<BinaryWriter> _workingPackets = [];
    public bool IsActive { get; private set; }

    public static SingleplayerServer Instance => ModContent.GetInstance<SingleplayerServer>();

    public int Netmode { get; private set; } = 1;

    public void AddPacket(BinaryWriter p) {
        _nextPackets.Add(p);
    }

    public override void PreUpdateEntities() {
        int oldNetmode = Main.netMode;
        IsActive = true;
        // Swap packet lists so we dont modify the working collection on accident when receiving packets.
        Utils.Swap(ref _nextPackets, ref _workingPackets);
        _nextPackets.Clear();

        try {
            foreach (var packet in _workingPackets) {
                BinaryReader reader = new BinaryReader(packet.BaseStream);
                reader.BaseStream.Position = 0L;

                int fromNetmode = reader.ReadByte();

                byte packetId = reader.ReadByte();

                if (fromNetmode == NetmodeID.MultiplayerClient) {
                    Main.netMode = Netmode = NetmodeID.Server;
                }
                else {
                    Main.netMode = Netmode = NetmodeID.MultiplayerClient;
                }

                try {
                    PacketSystem.Get(packetId).Receive(reader, Main.myPlayer);
                }
                catch (Exception ex) {
                    Mod.Logger.Error(ex);
                }
                packet.Dispose();
            }
        }
        catch (Exception ex) {
            Mod.Logger.Error(ex);
        }

        IsActive = false;
        Main.netMode = oldNetmode;

        _workingPackets.Clear();
    }

    public override void ClearWorld() {
        foreach (var p in _workingPackets) {
            p.Dispose();
        }
    }
}
