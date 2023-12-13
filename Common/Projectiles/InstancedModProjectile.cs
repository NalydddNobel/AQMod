using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles;

internal abstract class InstancedModProjectile : ModProjectile {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    public InstancedModProjectile(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}