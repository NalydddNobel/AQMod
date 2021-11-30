using AQMod.Common.Graphics.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Graphics.DrawTypes
{
    public class MonoParticleDraw : IDrawType
    {
        private readonly MonoParticle _particle;

        public MonoParticleDraw(MonoParticle particle)
        {
            _particle = particle;
        }

        void IDrawType.RunDraw()
        {
            Main.spriteBatch.Draw(_particle.Texture, _particle.position - Main.screenPosition, _particle.frame, _particle.color, _particle.rotation, _particle.origin, _particle.scale, SpriteEffects.None, 0f);
        }
    }
}
