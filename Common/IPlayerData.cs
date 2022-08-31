using Terraria;

namespace Aequus.Common
{
    internal interface IPlayerData
    {
        bool NeedsSyncing(AequusPlayer aequus, AequusPlayer c);
    }
}