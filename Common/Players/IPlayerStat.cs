using Terraria;

namespace Aequus.Common.Players
{
    public interface IPlayerStat
    {
        void Initalize(Player player, AequusPlayer aequus)
        {
        }
        void ResetEffects(Player player, AequusPlayer aequus)
        {
            Clear();
        }
        void Clear();
    }
}