namespace Aequus.Common.Projectiles;

internal abstract class InstancedModProjectile : ModProjectile {
    private readonly System.String _name;
    private readonly System.String _texture;

    public override System.String Name => _name;

    public override System.String Texture => _texture;

    protected override System.Boolean CloneNewInstances => true;

    public InstancedModProjectile(System.String name, System.String texture) {
        _name = name;
        _texture = texture;
    }
}