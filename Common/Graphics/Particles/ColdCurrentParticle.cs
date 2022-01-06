using AQMod.Assets;
using AQMod.Common.Graphics.DrawTypes;
using AQMod.Common.Graphics.SceneLayers;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Graphics.Particles
{
    public sealed class ColdCurrentParticle : MonoParticle
    {
        public override Texture2D Texture => AQTextures.Particles[ParticleTex.WindParticle];

        public ColdCurrentParticle(Vector2 position, Vector2 velocity = default(Vector2), Color color = default(Color), float scale = 1) : base(position, velocity, color, scale)
        {
        }

        public override void OnAdd()
        {
            frame = new Rectangle(0, 0, 38, 26);
            origin = frame.Size() / 2f;
        }

        public override void Draw()
        {
            WindLayer.AddToCurrentList(new MonoParticleDraw(this));
        }
    }
}