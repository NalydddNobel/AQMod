namespace Aequus.Core;

public static class Cull2D {
    public static System.Boolean Vector2(Vector2 input, System.Int32 paddingX = 32, System.Int32 paddingY = 32) {
        return input.X > Main.screenPosition.X - paddingX
            && input.X < Main.screenPosition.X + Main.screenWidth + paddingX
            && input.Y > Main.screenPosition.Y - paddingY
            && input.Y < Main.screenPosition.Y + Main.screenHeight + paddingY;
    }
    public static System.Boolean Rectangle(Rectangle input, System.Int32 paddingX = 32, System.Int32 paddingY = 32) {
        return input.X + input.Width > Main.screenPosition.X - paddingX
            && input.X < Main.screenPosition.X + Main.screenWidth + paddingX
            && input.Y + input.Height > Main.screenPosition.Y - paddingY
            && input.Y < Main.screenPosition.Y + Main.screenHeight + paddingY;
    }
}