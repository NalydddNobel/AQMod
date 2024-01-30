namespace Aequus.Common.Tiles;
internal class InstancedModTile : ModTile {
    internal readonly System.String _name;
    internal readonly System.String _texture;

    public override System.String Name => _name;

    public override System.String Texture => _texture;

    public InstancedModTile(System.String name, System.String texture) {
        _name = name;
        _texture = texture;
    }
}