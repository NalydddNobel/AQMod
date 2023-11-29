using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Core.Networking;

public partial class PacketSystem : ModSystem {
    public static readonly List<PacketHandler> Handlers = new();

    public static void Register(PacketHandler handler) {
        handler.Type = (byte)Handlers.Count;
        Handlers.Add(handler);
    }

    public override void Unload() {
        Handlers.Clear();
    }

    public static void HandlePacket(BinaryReader reader, int whoAmI) {
        var type = reader.ReadByte();
        if (Handlers.IndexInRange(type)) {
            Handlers[type].Receive(reader, whoAmI);
        }
    }

    public static T Get<T>() where T : PacketHandler {
        return ModContent.GetInstance<T>();
    }
}