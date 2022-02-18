using AQMod.Assets;
using AQMod.Common.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Effects.Particles
{
    public class EmberParticleSubtractColorUsingScale : MonoParticle
    {
        public override Texture2D Texture => AQTextures.Particles[ParticleTex.MonoEmber];

        public EmberParticleSubtractColorUsingScale(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1f) : base(position, velocity, color, scale)
        {
        }

        public override Color GetColor()
        {
            var c = base.GetColor();
            int subtract = (int)((1f - scale) * 255f);
            if (subtract > 0)
            {
                return c;
            }
            return new Color(c.R - subtract, c.G - subtract, c.B - subtract, c.A - subtract);
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 30 * Main.rand.Next(3), 30, 30);
            origin = frame.Size() / 2f;
        }
    }
}