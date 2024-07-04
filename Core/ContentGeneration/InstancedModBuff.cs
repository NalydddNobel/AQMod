namespace AequusRemake.Core.ContentGeneration;

internal class InstancedModBuff(string name, string texture) : ModBuff {
    protected readonly string _name = name;
    protected readonly string _texture = texture;

    public override string Name => _name;

    public override string Texture => _texture;
}
