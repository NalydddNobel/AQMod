using AQMod.Assets;
using AQMod.Assets.Graphics.SceneLayers;
using AQMod.Assets.Textures;
using AQMod.Common.Graphics.DrawTypes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AQMod.Common.Graphics.Particles
{
    public sealed class ColdCurrentParticle : MonoParticle
    {
        public override Texture2D Texture => TextureCache.Particles[ParticleTex.WindParticle];

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
            HotAndColdCurrentLayer.AddToCurrentList(new MonoParticleDraw(this));
        }
    }
}