using System;
using Terraria.ModLoader;

namespace AQMod.Assets.PlayerLayers
{
    public abstract class PlayerHeadLayerWrapper
    {
        public virtual string Mod => "AQMod";

        public virtual string Name => GetType().Name;

        public abstract Action<PlayerHeadDrawInfo> Apply();

        public PlayerHeadLayer GetLayer()
        {
            return new PlayerHeadLayer(Mod, Name, Apply());
        }
    }
}