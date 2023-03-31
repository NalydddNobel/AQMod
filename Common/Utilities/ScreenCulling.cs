using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus
{
    public class ScreenCulling
    {
        public static Rectangle renderBox;

        public static bool ServerSafeInView(Vector2 vector, Rectangle myVisibleLocation)
        {
            return new Rectangle((int)vector.X - 1000, (int)vector.Y - 600, 2000, 1200).Intersects(myVisibleLocation);
        }

        public static bool OnScreenWorld(Vector2 position)
        {
            return OnScreen(position - Main.screenPosition);
        }
        public static bool OnScreen(Vector2 position)
        {
            return OnScreen(new Rectangle((int)position.X, (int)position.Y, 1, 1));
        }
        public static bool OnScreenWorld(Rectangle rectangle)
        {
            return OnScreen(new Rectangle(rectangle.X - (int)Main.screenPosition.X, rectangle.Y - (int)Main.screenPosition.Y, rectangle.Width, rectangle.Height));
        }
        public static bool OnScreen(Rectangle rectangle)
        {
            return renderBox.Intersects(rectangle);
        }

        public static void Prepare(int padding = 20)
        {
            renderBox = new Rectangle(-padding, -padding, Main.screenWidth + padding * 2, Main.screenHeight + padding * 2);
        }
    }
}