namespace Aequus.Common.Tiles;
internal class InstancedModTile : ModTile {
    internal readonly string _name;
    internal readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    public InstancedModTile(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}