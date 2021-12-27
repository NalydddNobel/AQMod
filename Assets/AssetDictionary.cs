using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace AQMod.Assets
{
    public sealed class AssetDictionary<TEnum, TAsset> :
        Dictionary<TEnum, TAsset>,
        IDisposable
        where TEnum : Enum where TAsset : class
    {
        private readonly string _path;
        private readonly HashSet<TEnum> _search;
        private readonly Dictionary<TEnum, Asset<TAsset>> _assets;

        public AssetDictionary(string path) : base()
        {
            _path = path;
            _search = new HashSet<TEnum>();
            _assets = new Dictionary<TEnum, Asset<TAsset>>();
        }

        public new TAsset this[TEnum type]
        {
            get => Request(type);
        }

        public TAsset Request(TEnum type, AssetRequestMode requestMode = AssetRequestMode.AsyncLoad)
        {
            if (!_search.Contains(type))
            {
                _search.Add(type);
                _assets.Add(type, ModContent.Request<TAsset>(_path + type.GetHashCode(), requestMode));
            }
            return _assets[type].Value;
        }

        void IDisposable.Dispose()
        {
            foreach (var asset in _assets)
            {
                if (asset.Value != null)
                    asset.Value.Dispose();
            }
            _assets.Clear();
            _search.Clear();
        }
    }
}