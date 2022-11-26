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
        PhysicsGunBlock,
        RequestGlimmerEvent,
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
        GravityChestPickupEffect,
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