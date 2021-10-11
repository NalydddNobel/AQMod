using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;

namespace AQMod.Effects.Screen
{
    public class ScreenShake : ScreenFX
    {
        private readonly float _intensityX = 0f;
        private readonly float _intensityY = 0f;

        private float _offsetX = 0f;
        private float _offsetY = 0f;
        private int _time;

        public ScreenShake(int time, int intensity)
        {
            _time = time;
            _intensityX = intensity;
            _intensityY = intensity;
        }

        public ScreenShake(int time, int intensityX, int intensityY)
        {
            _time = time;
            _intensityX = intensityX;
            _intensityY = intensityY;
        }

        public override bool UpdateBiomeVisuals => false;

        public override void Apply()
        {
            Main.screenPosition += new Vector2(_offsetX, _offsetY);
        }

        public override bool Update()
        {
            _time--;
            if (_time <= 0)
                return false;
            _offsetX = _random.NextFloat(-_intensityX, _intensityX);
            _offsetY = _random.NextFloat(-_intensityY, _intensityY);
            return true;
        }
    }
}