using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Common.Utilities
{
    public sealed class ScreenCulling
    {
        public static Rectangle renderBox;

        internal static bool OnScreenWorld(Vector2 position)
        {
            return OnScreen(position - Main.screenPosition);
        }
        internal static bool OnScreen(Vector2 position)
        {
            return OnScreen(new Rectangle((int)position.X, (int)position.Y, 1, 1));
        }
        internal static bool OnScreenWorld(Rectangle rectangle)
        {
            return OnScreen(new Rectangle(rectangle.X - (int)Main.screenPosition.X, rectangle.Y - (int)Main.screenPosition.Y, rectangle.Width, rectangle.Height));
        }
        internal static bool OnScreen(Rectangle rectangle)
        {
            return renderBox.Intersects(rectangle);
        }

        internal static void SetFluff(int padding = 20)
        {
            renderBox = new Rectangle(-padding, -padding, Main.screenWidth + padding * 2, Main.screenHeight + padding * 2);
        }
    }
}