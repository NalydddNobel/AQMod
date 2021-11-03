using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Effects.ScreenEffects
{
    public class BasicScreenShake : ScreenShakeFX
    {
        private readonly float _intensityX = 0f;
        private readonly float _intensityY = 0f;

        private float _offsetX = 0f;
        private float _offsetY = 0f;
        private int _time;

        public BasicScreenShake(int time, int intensity)
        {
            _time = time;
            _intensityX = intensity;
            _intensityY = intensity;
        }

        public BasicScreenShake(int time, int intensityX, int intensityY)
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

        public override void AdoptChannel(ScreenShakeFX effect)
        {
            var basicScreenShake = (BasicScreenShake)effect;
            _time = basicScreenShake._time;
        }
    }
}