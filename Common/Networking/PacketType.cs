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
        /// <summary>
        /// 0) string, Sync Context
        /// <para>1) int, NetID</para>
        /// <para>2) Vector2, Spawn Location</para>
        /// <para>3) float, AI[0], optional</para>
        /// <para>4) float, AI[1], optional</para>
        /// <para>5) float, AI[2], optional</para>
        /// <para>6) float, AI[3], optional</para>
        /// </summary>
        ClientNPCSpawn,
        Count,
    }
}