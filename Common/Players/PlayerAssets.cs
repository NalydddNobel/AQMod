using Aequus.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Aequus.Common.Players
{
    public sealed class PlayerAssets : ILoadable
    {
        public static LoadableAsset<Texture2D> FocusAura { get; private set; }
        public static LoadableAsset<Texture2D> FocusCircle { get; private set; }

        void ILoadable.Load(Mod mod)
        {
            FocusAura = LoadableAsset<Texture2D>.Aequus("Player/FocusAura", AssetRequestMode.ImmediateLoad);
            FocusCircle = LoadableAsset<Texture2D>.Aequus("Player/FocusCircle", AssetRequestMode.ImmediateLoad);
        }

        void ILoadable.Unload()
        {
            FocusAura = null;
            FocusCircle = null;
        }
    }
}
