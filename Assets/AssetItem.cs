using System;
using Terraria;

namespace AQMod.Assets
{
    public abstract class AssetItem<T> : IDisposable
    {
        protected Ref<T> _asset;
        protected bool _loaded;
        protected bool _loadedProperly;

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
        public bool Loaded()
        {
            return _loaded;
        }
        public bool LoadedProperly()
        {
            return _loadedProperly;
        }

        public void Dispose()
        {
            _loaded = false;
            _loadedProperly = false;
            if (_asset != null && _asset.Value is IDisposable d)
            {
                d.Dispose();
            }
            _asset = null;
        }
    }
}