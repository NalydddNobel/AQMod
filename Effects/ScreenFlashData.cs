using Microsoft.Xna.Framework;

namespace Aequus.Effects
{
    public class ScreenFlashData
    {
        public Vector2 FlashLocation;
        public float Intensity;
        public float MultiplyPerTick;

        public void Set(Vector2 location, float brightness, float multiplier = 0.75f)
        {
            FlashLocation = location;
            Intensity = brightness;
            MultiplyPerTick = multiplier;
        }

        public void Clear()
        {
            FlashLocation = default(Vector2);
            Intensity = 0f;
            MultiplyPerTick = 0f;
        }
    }
}