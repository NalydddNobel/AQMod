namespace Aequus.Core.ContentGeneration;

[Autoload(false)]
internal abstract class InstancedModNPC : ModNPC {
    protected readonly string _name;
    protected readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    public InstancedModNPC(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}
