namespace Aequus.Common.Networking
{
    public enum PacketType : byte
    {
        Unused,
        SyncNecromancyOwnerTier,
        Unused_1,
        SoundQueue,
        DemonSiegeSacrificeStatus,
        RequestDemonSiege,
        RemoveDemonSiege,
        SyncDebuffs,
        Count,
    }
}