using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class AfterImageMonoParticle : TrailMonoParticle
    {
        public AfterImageMonoParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1, float rotation = 0, int trailLength = 10) : base(position, velocity, color, scale, rotation, trailLength)
        {
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            for (int i = 0; i < oldPos.Length; i++)
            {
                if (oldPos[i] == Vector2.Zero)
                    break;
                var p = AequusHelpers.CalcProgress(oldPos.Length, i);
                spritebatch.Draw(Texture, oldPos[i] - Main.screenPosition, frame, ParticleColor, Rotation, origin, Scale, SpriteEffects.None, 0f);
            }
            base.Draw(ref settings, spritebatch);
        }
    }
}