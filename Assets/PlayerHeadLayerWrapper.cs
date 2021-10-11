using Terraria.ModLoader;

namespace AQMod.Assets
{
    public abstract class PlayerHeadLayerWrapper
    {
        protected virtual string Mod => "AQMod";
        protected virtual string Name => GetType().Name;
        protected abstract void Draw(PlayerHeadDrawInfo info);

        public PlayerHeadLayer GetLayer()
        {
            return new PlayerHeadLayer(Mod, Name, Draw);
        }
    }
}