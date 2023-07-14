using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;

namespace Aequus.Common.Net {
    public abstract class PacketHandler : ModType {
        #if DEBUG
        public byte Type => (byte)LegacyPacketType;
        public abstract PacketType LegacyPacketType { get; }
        #else
        public byte Type => (byte)LegacyPacketType;
        public abstract PacketType LegacyPacketType { get; }
        //public byte Type { get; internal set; }
        #endif

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

        [Obsolete("Out of date. Replaced with 'Receive(BinaryReader, Int32)'")]
        public virtual void Receive(BinaryReader reader) {
        }
        public virtual void Receive(BinaryReader reader, int sender) {
            Receive(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModPacket GetPacket() {
            var packet = Aequus.Instance.GetPacket();
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
}