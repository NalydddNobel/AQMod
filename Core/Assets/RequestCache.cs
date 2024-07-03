using ReLogic.Content;
using System.Linq;

namespace Aequu2.Core.Assets;

public class RequestCache<T> where T : class {
    public readonly string Path;

    private string _modPath;
    public string ModPath =>
            // "Aequu2/" is 7 characters long
            _modPath ??= Path[7..];

    private string _name;
    public string Name => _name ??= Path.Split('/').Last().Trim();

    protected Asset<T> _asset;
    public Asset<T> Asset => _asset ??= ModContent.Request<T>(Path, AssetRequestMode.ImmediateLoad);

    public T Value => Asset.Value;

    public RequestCache(string path) {
        Path = path;
    }

    public Asset<T> Preload() {
        return _asset ??= ModContent.Request<T>(Path, AssetRequestMode.AsyncLoad);
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