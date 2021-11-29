using AQMod.Assets;
using AQMod.Assets.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Graphics.ParticlesLayers
{
    public class MonoParticleEmber : MonoParticle
    {
        public override Texture2D Texture => TextureCache.Particles[ParticleTex.MonoEmber];

        public MonoParticleEmber(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f) : base(position, velocity, color, scale)
        {
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 30 * Main.rand.Next(3), 30, 30);
            origin = frame.Size() / 2f;
        }
    }
}