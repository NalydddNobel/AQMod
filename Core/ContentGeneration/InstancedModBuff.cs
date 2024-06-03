namespace Aequus.Core.ContentGeneration;

public class InstancedModBuff : ModBuff {
    protected readonly string _name;
    protected readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    public InstancedModBuff(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}
