using AQMod.Content.Players;
using Terraria;

namespace AQMod.Items
{
    internal interface IUpdateVanity
    {
        void UpdateVanitySlot(Player player, AQPlayer aQPlayer, PlayerDrawEffects drawEffects, int i);
    }
}