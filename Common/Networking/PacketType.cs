namespace Aequus.Common.Networking
{
    public enum PacketType : byte
    {
        Unused,
        SyncNecromancyOwnerTier,
        SyncAequusPlayer,
        SoundQueue,
        DemonSiegeSacrificeStatus,
        RequestDemonSiege,
        RemoveDemonSiege,
        SyncDebuffs,
        SyncRecyclingMachine_CauseForSomeReasonNetRecieveIsntWorkingOnTileEntities,
        GiveoutEnemySoul,
        Count,
    }
}