using Microsoft.Xna.Framework;
using Terraria;

namespace AQMod.Effects.ScreenEffects
{
    public class OmegaStariteScreenShake : ScreenShakeFX
    {
        private float _intensityY;
        private float _intensityYLerp;
        private readonly byte _shakeTime;
        private float _offsetY;

        public OmegaStariteScreenShake(int intensity, float lerp = 0.02f, int shakeTime = 2)
        {
            _intensityY = intensity;
            _intensityYLerp = 0.02f;
            _shakeTime = 2;
        }

        public override bool UpdateBiomeVisuals => false;

        public override void AdoptChannel(ScreenShakeFX effect)
        {
            var o = (OmegaStariteScreenShake)effect;
            _intensityY = o._intensityY;
            _intensityYLerp = o._intensityYLerp;
        }

        public override void Apply()
        {
            Main.screenPosition += new Vector2(0f, _offsetY);
        }

        public override bool Update()
        {
            if (_intensityY > 0f)
            {
                _intensityY = MathHelper.Lerp(_intensityY, 0f, _intensityYLerp);
                if (_intensityY <= 0.5f)
                {
                    return false;
                }
                _offsetY = _intensityY * (((int)Main.GameUpdateCount % _shakeTime * 2) / _shakeTime == 0 ? -1 : 1);
                return true;
            }
            return false;
        }
    }
}