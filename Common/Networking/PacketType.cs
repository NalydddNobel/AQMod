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
        Count,
    }
}