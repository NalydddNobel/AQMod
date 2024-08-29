namespace Aequus.Common.ContentTemplates.Generic;

[Autoload(false)]
internal class InstancedProjectile(string name, string texture) : ModProjectile {
    public override string Name => name;

    public override string Texture => texture;

    protected override bool CloneNewInstances => true;
}
