using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aequus.Core.Networking;

public abstract class PacketHandler : ModType {
    public Byte Type { get; internal set; }

    protected sealed override void Register() {
        if (GetType().GetMethod("Send", BindingFlags.Public | BindingFlags.Instance) == null) {
            // Require all inherited Packet Handlers to have a "Send" method.
            throw new Exception($"A public instanced 'Send' method was not found in {Name}.");
        }
        Aequus.RegisterPacket(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    public abstract void Receive(BinaryReader reader, Int32 sender);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ModPacket GetPacket() {
        var packet = Aequus.Instance.GetPacket();
        packet.Write(Type);
        return packet;
    }
}