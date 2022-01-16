using AQMod.Assets;
using AQMod.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Effects.Particles
{
    public class SparkleParticle : MonoParticle
    {
        public override Texture2D Texture => AQTextures.Particles[ParticleTex.Sparkle];

        public SparkleParticle(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f) : base(position, velocity, color, scale)
        {
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 0, 49, 49);
            origin = frame.Size() / 2f;
        }

        public override bool Update()
        {
            velocity *= 0.9f;
            float velo = velocity.Length();
            scale -= 0.05f - velo / 1000f;
            if (scale <= 0.1f)
                return false;
            Lighting.AddLight(position, color.ToVector3() * 0.5f);
            position += velocity;
            return true;
        }
    }
}