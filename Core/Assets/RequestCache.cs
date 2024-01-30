using ReLogic.Content;
using System.Linq;

namespace Aequus.Core.Assets;

public class RequestCache<T> where T : class {
    public readonly string Path;
    public readonly string ModPath;

    private string _name;
    public string Name { get => _name ??= Path.Split('/').Last().Trim(); }

    protected Asset<T> _asset;
    public Asset<T> Asset => _asset ??= ModContent.Request<T>(Path, AssetRequestMode.ImmediateLoad);

    protected Ref<T> _ref;
    public Ref<T> Ref => _ref ??= new(Asset.Value);

    public T Value => Asset.Value;

    public RequestCache(string path) {
        Path = path;
        ModPath = path[7..];
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