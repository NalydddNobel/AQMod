using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Aequus.Particles
{
    public class ParticleTextureAssets : ILoadable
    {
        public static Asset<Texture2D> SquareParticle { get; private set; }

        void ILoadable.Load(Mod mod)
        {
        }

        void ILoadable.Unload()
        {
        }
    }
}
