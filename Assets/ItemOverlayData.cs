using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public abstract class ItemOverlayData
    {
        public virtual void DrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
        }

        public virtual void DrawHeld(Player player, AQPlayer aQPlayer, Item item, PlayerDrawInfo info)
        {
        }

        public virtual bool PreDrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            return false;
        }

        public virtual void PostDrawInventory(Player player, AQPlayer aQPlayer, Item item, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
        }
    }
}