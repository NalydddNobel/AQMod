using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aequus.Core.Networking;

public abstract class PacketHandler : ModType {
    public byte Type { get; internal set; }

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

    public abstract void Receive(BinaryReader reader, int sender);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ModPacket GetPacket() {
        var packet = Aequus.Instance.GetPacket();
        packet.Write(Type);
        return packet;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// Casts an <see cref="int"/> into a <see cref="byte"/>. Clamped between 0 and 255.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected byte AsClampedByte(int value) {
        return (byte)Math.Clamp(value, byte.MinValue, byte.MaxValue);
    }
}