using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace AQMod.Assets
{
    public static class AQGraphics
    {
        internal delegate void DrawMethod(Texture2D texture, Vector2 position, Rectangle? frame, Color color, float scale, Vector2 origin, float rotation, SpriteEffects effects, float layerDepth);

        public static class Data
        {
            public static bool AllowedToUseAssets => Main.netMode != NetmodeID.Server && !AQMod.IsLoading;
            /// <summary>
            /// Gets the center of the screen's draw coordinates
            /// </summary>
            internal static Vector2 ScreenCenter => new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);
            /// <summary>
            /// Gets the center of the screen's world coordinates
            /// </summary>
            internal static Vector2 WorldScreenCenter => new Vector2(Main.screenPosition.X + (Main.screenWidth / 2f), Main.screenPosition.Y + Main.screenHeight / 2f);
            /// <summary>
            /// The world view point matrix
            /// </summary>
            internal static Matrix WorldViewPoint
            {
                get
                {
                    var graphics = Main.graphics.GraphicsDevice;
                    var screenZoom = Main.GameViewMatrix.Zoom;
                    int width = graphics.Viewport.Width;
                    int height = graphics.Viewport.Height;

                    var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                        Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                        Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
                    var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                    return zoom * projection;
                }
            }
            internal static Vector2 TileZero => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
        }
    }
}