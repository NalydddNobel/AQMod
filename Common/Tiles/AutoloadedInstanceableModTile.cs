using Aequus.Core.ContentGeneration;

namespace Aequus.Common.Tiles;

[Autoload(true)]
internal abstract class AutoloadedInstanceableModTile : InstancedModTile {
    public AutoloadedInstanceableModTile() : this(null, null) { }

    public AutoloadedInstanceableModTile(string? nameOverride = null, string? textureOverride = null) : base(nameOverride, textureOverride) {
        _name ??= GetType().Name;
        _texture ??= GetType().GetFilePath();
    }
}