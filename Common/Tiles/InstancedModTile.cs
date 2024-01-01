namespace Aequus.Common.Tiles;

internal class InstancedModTile : ModTile {
    internal string _name { get; set; }
    internal string _texture { get; set; }

    public override string Name => _name;

    public override string Texture => _texture;

    public InstancedModTile(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}