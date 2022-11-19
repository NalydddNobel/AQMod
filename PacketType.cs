namespace Aequus
{
    public enum PacketType : byte
    {
        RequestTileSectionFromServer,
        SyncNecromancyOwner,
        SyncAequusPlayer,
        SyncSound,
        DemonSiegeSacrificeStatus,
        StartDemonSiege,
        RemoveDemonSiege,
        Unused,
        Unused2,
        Unused3,
        Unused4,
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
        RequestAnalysisQuest,
        SpawnShutterstockerClip,
        AnalysisRarity,
        ZombieConvertEffects,
        Count
    }

    public enum SoundPacket : byte
    {
        InflictBleeding,
        InflictBurning,
        InflictBurning2,
        InflictNightfall,
        Count
    }
}