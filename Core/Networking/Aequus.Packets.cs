using Aequus.Core.Networking;
using System;
using System.Collections.Generic;
using System.IO;

namespace Aequus;

public partial class Aequus {
    public static readonly List<PacketHandler> PacketHandlers = new();

    public static void RegisterPacket(PacketHandler handler) {
        var limit = byte.MaxValue;
        if (PacketHandlers.Count > limit) {
            throw new IndexOutOfRangeException($"Packet Handler limit ({limit}) has been reached.");
        }

        handler.Type = (byte)PacketHandlers.Count;
        PacketHandlers.Add(handler);
    }

    private static void UnloadPackets() {
        PacketHandlers.Clear();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        var type = reader.ReadByte();
        if (PacketHandlers.IndexInRange(type)) {
            PacketHandlers[type].Receive(reader, whoAmI);
        }
    }

    public static T GetPacket<T>() where T : PacketHandler {
        return ModContent.GetInstance<T>();
    }
}