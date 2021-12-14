using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Items.DrawOverlays
{
    public interface IOverlayDrawWorld
    {
        bool PreDrawWorld(Item item, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI);
        void PostDrawWorld(Item item, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI);
    }
}