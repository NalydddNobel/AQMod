namespace Aequus.Common.ContentTemplates.Generic;

[Autoload(false)]
internal class InstancedNPC : ModNPC {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    public InstancedNPC(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}