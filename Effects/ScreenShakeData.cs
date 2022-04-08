using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus.Effects
{
    public class ScreenShakeData
    {
        public float Intensity;
        public float MultiplyPerTick;

        private Vector2 _shake;

        public void Update()
        {
            if (Intensity > 0f)
            {
                _shake = (Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(Intensity)).Floor();
                Intensity *= MultiplyPerTick;
            }
            else
            {
                Clear();
            }
        }

        public Vector2 GetScreenOffset()
        {
            return _shake;
        }

        public void Set(float intensity, float multiplier = 0.9f)
        {
            Intensity = intensity;
            MultiplyPerTick = multiplier;
        }
        public void Clear()
        {
            Intensity = 0f;
            MultiplyPerTick = 0.9f;
            _shake = new Vector2();
        }
    }
}