namespace Aequus.Core.Utilities;

public static class Cull2D {
    public static bool Vector2(Vector2 input, int paddingX = 32, int paddingY = 32) {
        return input.X > Main.screenPosition.X - paddingX
            && input.X < Main.screenPosition.X + Main.screenWidth + paddingX
            && input.Y > Main.screenPosition.Y - paddingY
            && input.Y < Main.screenPosition.Y + Main.screenHeight + paddingY;
    }
    public static bool Rectangle(Rectangle input, int paddingX = 32, int paddingY = 32) {
        return input.X + input.Width > Main.screenPosition.X - paddingX
            && input.X < Main.screenPosition.X + Main.screenWidth + paddingX
            && input.Y + input.Height > Main.screenPosition.Y - paddingY
            && input.Y < Main.screenPosition.Y + Main.screenHeight + paddingY;
    }

    public static bool Tile(int tileX, int tileY, int screenPixelsPaddingX = 32, int screenPixelsPaddingY = 32) {
        return Vector2(new Vector2(tileX * 16f + 8f, tileY * 16f + 8f), screenPixelsPaddingX, screenPixelsPaddingY);
    }
}