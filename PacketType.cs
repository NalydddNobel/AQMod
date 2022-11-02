using System.IO;

namespace Aequus
{
    public enum PacketType : byte
    {
        RequestTileSectionFromServer,
        SyncNecromancyOwner,
        SyncAequusPlayer,
        SyncZombieRecruitSound,
        DemonSiegeSacrificeStatus,
        StartDemonSiege,
        RemoveDemonSiege,
        Unused,
        SyncRecyclingMachine,
        CandleSouls,
        SoulDamage,
        ExporterQuestsCompleted,
        SpawnOmegaStarite,
        GlimmerStatus,
        SyncNecromancyNPC,
        SyncDronePoint,
        CarpenterBountiesCompleted,
        AequusTileSquare,
        OnKillEffect,
        ApplyNameTagToNPC,
        RequestChestItems,
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