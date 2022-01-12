using AQMod.Content.Players;
using Terraria;

namespace AQMod.Items
{
    internal interface IUpdateEquipVisuals
    {
        void UpdateEquipVisuals(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i);
    }
}