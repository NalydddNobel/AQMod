using System.IO;

namespace Aequus.Common.Networking
{
    public enum PacketType : byte
    {
        RequestTileSectionFromServer,
        SyncNecromancyOwner,
        SyncAequusPlayer,
        SoundQueue,
        DemonSiegeSacrificeStatus,
        StartDemonSiege,
        RemoveDemonSiege,
        SyncAequusNPC,
        SyncRecyclingMachine,
        GiveoutEnemySouls,
        SoulDamage,
        SetExporterQuestsCompleted,
        SpawnOmegaStarite,
        GlimmerEventUpdate,
        SyncNecromancyNPC,
        SyncDronePoint,
        CarpenterBountiesCompleted,
        Count,
    }

    public static class PacketTypeExtensions
    {
        public static void Write(this BinaryWriter writer, PacketType packetType)
        {
            writer.Write((byte)packetType);
        }
        public static PacketType ReadPacketType(this BinaryReader reader)
        {
            return (PacketType)reader.ReadByte();
        }
    }
}