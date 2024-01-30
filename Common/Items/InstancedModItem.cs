namespace Aequus.Common.Items;

internal abstract class InstancedModItem : ModItem {
    protected readonly System.String _name;
    protected readonly System.String _texture;

    public override System.String Name => _name;

    public override System.String Texture => _texture;

    protected override System.Boolean CloneNewInstances => true;

    private void TryAlternativeTexturePaths(ref System.String texture) {
        if (ModContent.HasAsset(texture)) {
            return;
        }

        System.Int32 i = texture.LastIndexOf('/');
        System.String name = texture[(i + 1)..];
        System.String path = texture[..i];

        System.String itemsFolder = path + "/Items/" + name;
        if (ModContent.HasAsset(itemsFolder)) {
            texture = itemsFolder;
            return;
        }
    }

    public InstancedModItem(System.String name, System.String texture) {
        _name = name;
        _texture = texture;
        if (!Main.dedServ) {
            TryAlternativeTexturePaths(ref _texture);
        }
    }
}