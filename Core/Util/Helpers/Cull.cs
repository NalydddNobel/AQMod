namespace AequusRemake.Core.Util.Helpers;

public sealed class Cull {
    public static bool ClipXY(Vector2 input, int paddingX = 32, int paddingY = 32) {
        return input.X > Main.screenPosition.X - paddingX
            && input.X < Main.screenPosition.X + Main.screenWidth + paddingX
            && input.Y > Main.screenPosition.Y - paddingY
            && input.Y < Main.screenPosition.Y + Main.screenHeight + paddingY;
    }

    public static bool ClipXYWH(Rectangle input, int paddingX = 32, int paddingY = 32) {
        return ClipXYWH(input.X, input.Y, input.Width, input.Height, paddingX, paddingY);
    }

    public static bool ClipXYWH(Vector2 input, int width, int height, int paddingX = 32, int paddingY = 32) {
        return ClipXYWH((int)input.X, (int)input.Y, width, height, paddingX, paddingY);
    }

    public static bool ClipXYWH(int x, int y, int width, int height, int paddingX = 32, int paddingY = 32) {
        return x + width > Main.screenPosition.X - paddingX
            && x < Main.screenPosition.X + Main.screenWidth + paddingX
            && y + height > Main.screenPosition.Y - paddingY
            && y < Main.screenPosition.Y + Main.screenHeight + paddingY;
    }

    /// <returns>Whether a tile is in a screen clipping zone.</returns>
    public static bool ClipIJ(int tileX, int tileY, int screenPixelsPaddingX = 32, int screenPixelsPaddingY = 32) {
        return ClipXYWH(tileX * 16 + 8, tileY * 16 + 8, 16, 16, screenPixelsPaddingX, screenPixelsPaddingY);
    }
}