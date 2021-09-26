using System;
using Terraria.ModLoader;

namespace AQMod.Assets.PlayerLayers
{
    public abstract class PlayerLayerWrapper
    {
        public virtual string Mod => "AQMod";

        public virtual string Name => GetType().Name;

        public abstract Action<PlayerDrawInfo> Apply();

        public PlayerLayer GetLayer()
        {
            return new PlayerLayer(Mod, Name, Apply());
        }
    }
}