using ReLogic.Content;

namespace Aequus.Core.Assets;

public class RequestCache<T> where T : class {
    public readonly string Path;

    private string _modPath;
    public string ModPath {
        get {
            // "Aequus/" is 7 characters long
            return _modPath ??= Path[7..];
        }
    }

    protected Asset<T> _asset;
    public Asset<T> Asset => _asset ??= ModContent.Request<T>(Path, AssetRequestMode.ImmediateLoad);

    protected Ref<T> _ref;
    public Ref<T> Ref => _ref ??= new(Asset.Value);

    public T Value => Asset.Value;

    public RequestCache(string path) {
        Path = path;
    }

    public virtual void Unload() {
        _asset = null;
        _ref = null;
    }

    public static implicit operator Asset<T>(RequestCache<T> value) {
        return value.Asset;
    }
    public static implicit operator Ref<T>(RequestCache<T> value) {
        return value.Ref;
    }
    public static implicit operator T(RequestCache<T> value) {
        return value.Value;
    }
}