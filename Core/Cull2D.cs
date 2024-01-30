namespace Aequus.Core;

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
}