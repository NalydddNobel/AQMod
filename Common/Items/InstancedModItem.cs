using Terraria.ModLoader;

namespace Aequus.Common.Items;

internal abstract class InstancedModItem : ModItem {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    public InstancedModItem(string name, string texture) {
        _name = name;
        _texture = texture;
    }
}