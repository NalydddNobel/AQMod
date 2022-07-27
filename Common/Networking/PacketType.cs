namespace Aequus.Common.Networking
{
    public enum PacketType : byte
    {
        Unused,
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
        Count,
    }
}