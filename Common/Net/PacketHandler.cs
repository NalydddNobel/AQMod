using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aequus.Common.Net;

public abstract class PacketHandler : ModType {
    public byte Type => (byte)LegacyPacketType;
    public abstract PacketType LegacyPacketType { get; }

    protected sealed override void Register() {
        if (GetType().GetMethod("Send", BindingFlags.Public | BindingFlags.Instance) == null) {
            // Require all inherited Packet Handlers to have a "Send" method.
            throw new Exception($"A public instanced 'Send' method was not found in {Name}.");
        }
        PacketSystem.Register(this);
    }

    public sealed override void SetupContent() {
        SetStaticDefaults();
    }

    protected void SendPacket(BinaryWriter p, int toClient = -1, int ignoreClient = -1) {
        if (p is ModPacket netPacket) {
            netPacket.Send(toClient, ignoreClient);
        }
        else {
            ModContent.GetInstance<SingleplayerServer>().AddPacket(p);
        }
    }

    [Obsolete("Out of date. Replaced with 'Receive(BinaryReader, Int32)'")]
    public virtual void Receive(BinaryReader reader) {
    }
    public virtual void Receive(BinaryReader reader, int sender) {
        Receive(reader);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("Replaced with new GetPacket, returning a BinaryWriter instead of ModPacket to support the Singleplayer Server system.")]
    public ModPacket GetLegacyPacket(int capacity = byte.MaxValue + 1) {
        var packet = Aequus.Instance.GetPacket(capacity);
        packet.Write(Type);
        return packet;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BinaryWriter GetPacket(int capacity = byte.MaxValue + 1) {
        BinaryWriter packet;

        if (Main.netMode == NetmodeID.SinglePlayer || SingleplayerServer.Instance.IsActive) {
            packet = new BinaryWriter(new MemoryStream(capacity));
            packet.Write((byte)SingleplayerServer.Instance.Netmode);
        }
        else {
            packet = Aequus.Instance.GetPacket(capacity);
        }

        packet.Write(Type);
        return packet;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// Casts an <see cref="Int32"/> into a <see cref="Byte"/>. Clamped between 0 and 255.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected byte AsClampedByte(int value) {
        return (byte)Math.Clamp(value, byte.MinValue, byte.MaxValue);
    }
}