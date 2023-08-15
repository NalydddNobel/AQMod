using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common;
public abstract class TemplateAsset<T> where T : class {
    public readonly string Path;

    protected Asset<T> _asset;
    public Asset<T> Asset => _asset ??= ModContent.Request<T>(Path, AssetRequestMode.ImmediateLoad);

    protected Ref<T> _ref;
    public Ref<T> Ref => _ref ??= new(Asset.Value);

    public T Value => Asset.Value;

    public TemplateAsset(string path) {
        Path = path;
    }

    public virtual void Unload() {
        _asset = null;
        _ref = null;
    }

    public static implicit operator Asset<T>(TemplateAsset<T> value) {
        return value.Asset;
    }
    public static implicit operator Ref<T>(TemplateAsset<T> value) {
        return value.Ref;
    }
    public static implicit operator T(TemplateAsset<T> value) {
        return value.Value;
    }
}