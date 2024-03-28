namespace Aequus.Core.ContentGeneration;

[Autoload(false)]
internal abstract class InstancedModGore : ModGore {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    public InstancedModGore(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}