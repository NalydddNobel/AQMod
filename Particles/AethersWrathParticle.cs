using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;

namespace Aequus.Particles
{
    public class AethersWrathParticle : BaseTrailParticle
    {
        public AethersWrathParticle(Vector2 position, Vector2 velocity, Color color = default(Color), float scale = 1, float rotation = 0, int trailLength = 10) : base(position, velocity, color, scale, rotation, trailLength)
        {
            SetTexture(ParticleTextures.monoParticle);
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            base.Update(ref settings);
            Velocity.X *= 1.02f;
            Velocity.Y += 0.1f;
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            for (int i = 0; i < oldPos.Length; i++)
            {
                if (oldPos[i] == Vector2.Zero)
                    break;
                var p = AequusHelpers.CalcProgress(oldPos.Length, i);
                spritebatch.Draw(texture, oldPos[i] - Main.screenPosition, frame, GetParticleColor(ref settings) * 0.2f * p, Rotation, origin, Scale, SpriteEffects.None, 0f);
            }
            base.Draw(ref settings, spritebatch);
        }
    }
}