using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace AQMod.Assets.Effects
{
    public sealed class EffectCache : ILoadable
    {
        public static Asset<Effect> Trailshader { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            Trailshader = ModContent.Request<Effect>("AQMod/Assets/Effects/Trails/Trailshader", AssetRequestMode.AsyncLoad);
        }

        void ILoadable.Unload()
        {
            Trailshader = null;
        }
    }
}