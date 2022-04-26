using System;
using Terraria.ModLoader;

namespace Aequus.Common.Networking
{
    public sealed class PacketSender
    {
        public static void Send(Action<ModPacket> action, PacketType type, int capacity = 256, int to = -1, int ignore = -1)
        {
            var packet = Aequus.Instance.GetPacket(capacity);
            packet.Write((byte)type);
            action(packet);
            packet.Send(to, ignore);
        }
    }
}