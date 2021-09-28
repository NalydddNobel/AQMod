using Terraria;

namespace AQMod.Assets
{
    public abstract class AssetItem<T>
    {
        protected Ref<T> _asset;
        protected bool _loaded;

        public abstract void LoadAsset();
        public Ref<T> GetAsset()
        {
            LoadAsset();
            return _asset;
        }
        public T GetValue()
        {
            return GetAsset().Value;
        }
    }
}