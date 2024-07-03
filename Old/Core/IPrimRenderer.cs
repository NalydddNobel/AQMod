namespace Aequu2.Old.Core;

public interface IPrimRenderer {
    void Draw(Vector2[] arr, float uvAdd = 0f, float uvMultiplier = 1f);

    public static Matrix WorldViewPoint {
        get {
            GraphicsDevice graphics = Main.graphics.GraphicsDevice;
            Vector2 screenZoom = Main.GameViewMatrix.Zoom;
            int width = graphics.Viewport.Width;
            int height = graphics.Viewport.Height;

            var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
            var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            return zoom * projection;
        }
    }
}