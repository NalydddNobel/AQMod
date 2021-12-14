using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Items.DrawOverlays
{
    public interface IOverlayDrawInventory
    {
        bool PreDrawInv(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale);
        void PostDrawInv(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale);
    }
}