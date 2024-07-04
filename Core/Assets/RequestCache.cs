using ReLogic.Content;
using System.Linq;

namespace AequusRemake.Core.Assets;

public class RequestCache<T>(string path) where T : class {
    public readonly string Path = path;

    //private string _fullPath;
    public string FullPath => $"{nameof(AequusRemake)}/{Path}";

    private string _name;
    public string Name => _name ??= FullPath.Split('/').Last().Trim();

    protected Asset<T> _asset;

    public Asset<T> Asset => _asset ??= ModContent.Request<T>(FullPath, AssetRequestMode.ImmediateLoad);

    public T Value => Asset.Value;

    public Asset<T> Preload() {
        return _asset ??= ModContent.Request<T>(FullPath, AssetRequestMode.AsyncLoad);
    }

    public virtual void Unload() {
        _asset = null;
    }

    public static implicit operator Asset<T>(RequestCache<T> value) {
        return value.Asset;
    }

    public static implicit operator T(RequestCache<T> value) {
        return value.Value;
    }
}