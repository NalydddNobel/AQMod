using ReLogic.Content;
using Terraria.ModLoader;

namespace Aequus.Graphics
{
    public sealed class LoadableAsset<T> where T : class
    {
        private Asset<T> _asset;
        private readonly string RequestPath;
        private readonly AssetRequestMode RequestMode;

        public Asset<T> Asset
        {
            get
            {
                if (_asset == null)
                {
                    return LoadAsset();
                }
                return _asset;
            }
        }
        public T Value
        {
            get
            {
                if (_asset == null && LoadAsset() == null)
                {
                    return null;
                }
                return _asset.Value;
            }
        }

        public static LoadableAsset<T> Aequus(string name, AssetRequestMode requestMode = AssetRequestMode.AsyncLoad)
        {
            return new LoadableAsset<T>("Aequus/Assets/" + name, requestMode);
        }

        public LoadableAsset(string name, AssetRequestMode requestMode = AssetRequestMode.AsyncLoad)
        {
            _asset = null;
            RequestPath = name;
            RequestMode = requestMode;
        }

        public Asset<T> LoadAsset()
        {
            _asset = ModContent.Request<T>(RequestPath, RequestMode);
            return _asset;
        }
    }
}