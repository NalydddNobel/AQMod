using Microsoft.Xna.Framework;

namespace Aequus.Graphics
{
    public class ScreenFlash
    {
        public Vector2 FlashLocation;
        public float Intensity;
        public float Multiplier;

        public void Set(Vector2 location, float brightness, float multiplier = 0.9f)
        {
            FlashLocation = location;
            Intensity = brightness;
            Multiplier = multiplier;
        }

        public void Clear()
        {
            FlashLocation = default(Vector2);
            Intensity = 0f;
            Multiplier = 0f;
        }
    }
}