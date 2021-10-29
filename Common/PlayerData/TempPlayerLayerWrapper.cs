using Terraria.ModLoader;

namespace AQMod.Common.PlayerData
{
    public abstract class TempPlayerLayerWrapper
    {
        public virtual string Mod => "AQMod";
        public virtual string Name => GetType().Name;
        public abstract void Draw(PlayerDrawInfo info);

        public PlayerLayer GetLayer()
        {
            return new PlayerLayer(Mod, Name, Draw);
        }
    }
}