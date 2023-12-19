namespace Aequus.Common.Buffs;

internal abstract class InstancedBuff : ModBuff {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    public InstancedBuff(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}