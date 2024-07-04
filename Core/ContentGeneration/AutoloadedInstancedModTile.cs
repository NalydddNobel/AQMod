namespace AequusRemake.Core.ContentGeneration;

[Autoload(true)]
internal abstract class AutoloadedInstancedModTile : InstancedModTile {
    public AutoloadedInstancedModTile() : this(null, null) { }

    public AutoloadedInstancedModTile(string nameOverride = null, string textureOverride = null) : base(nameOverride, textureOverride) {
        _name ??= GetType().Name;
        _texture ??= GetType().GetFilePath();
    }
}