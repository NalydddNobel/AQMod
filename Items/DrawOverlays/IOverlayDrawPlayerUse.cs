using Terraria;
using Terraria.DataStructures;

namespace AQMod.Items.DrawOverlays
{
    public interface IOverlayDrawPlayerUse
    {
        void DrawUse(Player player, AQPlayer aQPlayer, Item item, PlayerDrawSet info);
    }
}