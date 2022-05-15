namespace Aequus.Common.Networking
{
    public enum PacketType : byte
    {
        SyncNPCNetworkerGlobals,
        SyncNecromancyOwnerTier,
        SyncProjNetworkerGlobals,
        SoundQueue,
        DemonSiegeSacrificeStatus,
        RequestDemonSiege,
        RemoveDemonSiege,
        SyncDebuffs,
        Count,
    }
}