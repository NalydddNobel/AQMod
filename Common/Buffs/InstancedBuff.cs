namespace Aequus.Common.Buffs;

internal abstract class InstancedBuff : ModBuff {
    private readonly System.String _name;
    private readonly System.String _texture;

    public override System.String Name => _name;

    public override System.String Texture => _texture;

    public InstancedBuff(System.String name, System.String texture) {
        _name = name;
        _texture = texture;
    }
}