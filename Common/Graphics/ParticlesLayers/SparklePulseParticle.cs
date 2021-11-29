using AQMod.Assets;
using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AQMod.Common.Graphics.ParticlesLayers
{
    public class SparklePulseParticle : MonoParticle
    {
        public override Texture2D Texture => TextureCache.Particles[ParticleTex.Sparkle];
        private float _scale;
        public int timeLeft;
        private int _timeLeftMax;

        public SparklePulseParticle(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f, int lifeSpan = 120) : base(position, velocity, color, scale)
        {
            _scale = scale;
            timeLeft = lifeSpan;
            _timeLeftMax = lifeSpan;
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 0, 49, 49);
            origin = frame.Size() / 2f;
        }

        public override bool Update()
        {
            if (timeLeft > 0)
            {
                velocity *= 0.98f;
                timeLeft--;
                scale = _scale + (float)Math.Sin((timeLeft - _timeLeftMax) * 0.01f) * 0.2f;
                rotation += velocity.Length() * 0.01f;
            }
            else
            {
                velocity *= 0.9f;
                float velo = velocity.Length();
                scale -= 0.02f - velo / 1000f;
                rotation += velocity.Length() * 0.015f;
                if (scale <= 0.1f)
                    return false;
            }
            Lighting.AddLight(position, color.ToVector3() * 0.5f * scale);
            position += velocity.RotatedBy((float)Math.Sin((timeLeft - _timeLeftMax) * 0.02f + Main.GlobalTime * 0.125f + 125f) * 0.2f) * (0.6f + ((float)Math.Sin(Main.GlobalTime) + 1f) * 0.2f);
            return true;
        }
    }
}