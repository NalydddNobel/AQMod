using System;
using System.IO;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Common.Net {
    public abstract class PacketHandler : ModType
    {
        public byte Type => (byte)LegacyPacketType;
        public abstract PacketType LegacyPacketType { get; }

        protected sealed override void Register()
        {
            if (GetType().GetMethod("Send", BindingFlags.Public | BindingFlags.Instance) == null)
            {
                throw new Exception($"A public instanced 'Send' method was not found in {Name}.");
            }
            PacketSystem.Register(this);
        }

        public sealed override void SetupContent()
        {
            SetStaticDefaults();
        }

        public abstract void Receive(BinaryReader reader);

        public ModPacket GetPacket()
        {
            var packet = Aequus.Instance.GetPacket();
            packet.Write(Type);
            return packet;
        }
    }
}