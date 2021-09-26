using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Common
{
    public static class CameraManager
    {
        public static float ScreenShakeYMagnitude { get; set; } = 0f;
        public static float ScreenShakeYLerp { get; set; } = 0.1f;
        public static int ScreenShakeYOffsetTime = 6;
        private static float _screenShakeMagnitude = 0f;
        private static int _screenShakeTime = 0;

        public static void Shake(float magnitude, int time = 10)
        {
            _screenShakeMagnitude = magnitude;
            _screenShakeTime = time;
        }

        public static void Update()
        {
            if (ScreenShakeYMagnitude > 0f)
            {
                ScreenShakeYMagnitude = MathHelper.Lerp(ScreenShakeYMagnitude, 0f, ScreenShakeYLerp);
                if (ScreenShakeYMagnitude <= 0.5f)
                    ScreenShakeYMagnitude = 0f;
                Main.screenPosition += new Vector2(0f, ScreenShakeYMagnitude * (Main.GameUpdateCount % (ScreenShakeYOffsetTime * 2) / 2f == 0 ? -1 : 1));
            }
            if (_screenShakeTime > 0)
            {
                _screenShakeTime--;
                if (_screenShakeMagnitude > 0f)
                    Main.screenPosition += new Vector2(Main.rand.NextFloat(-_screenShakeMagnitude, _screenShakeMagnitude), Main.rand.NextFloat(-_screenShakeMagnitude, _screenShakeMagnitude));
                else
                    _screenShakeTime = 0;
            }
            else
            {
                _screenShakeTime = 0;
            }
        }

        public static Vector2 UpsideDown(Vector2 position)
        {
            return Main.player[Main.myPlayer].gravDir == -1 ? new Vector2(position.X, -position.Y + Main.screenHeight) : new Vector2(position.X, position.Y);
        }
    }
}