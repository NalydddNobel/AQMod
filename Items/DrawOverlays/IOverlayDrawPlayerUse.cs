using Terraria;
using Terraria.ModLoader;

namespace AQMod.Items.DrawOverlays
{
    public interface IOverlayDrawPlayerUse
    {
        void DrawUse(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info);
    }
}