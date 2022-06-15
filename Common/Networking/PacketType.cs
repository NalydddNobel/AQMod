namespace Aequus.Common.Networking
{
    public enum PacketType : byte
    {
        Unused,
        SyncNecromancyOwner,
        SyncAequusPlayer,
        SoundQueue,
        DemonSiegeSacrificeStatus,
        RequestDemonSiege,
        RemoveDemonSiege,
        SyncDebuffs,
        SyncRecyclingMachine_CauseForSomeReasonNetRecieveIsntWorkingOnTileEntities,
        GiveoutEnemySouls,
        SoulDamage,
        SetExporterQuestsCompleted,
        Count,
    }
}