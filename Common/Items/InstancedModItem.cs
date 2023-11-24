using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items;

internal abstract class InstancedModItem : ModItem {
    private readonly string _name;
    private readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    private void TryAlternativeTexturePaths(ref string texture) {
        if (ModContent.HasAsset(texture)) {
            return;
        }

        int i = texture.LastIndexOf('/');
        string name = texture[(i + 1)..];
        string path = texture[..i];

        string itemsFolder = path + "/Items/" + name;
        if (ModContent.HasAsset(itemsFolder)) {
            texture = itemsFolder;
            return;
        }
    }

    public InstancedModItem(string name, string texture) {
        _name = name;
        _texture = texture;
        if (!Main.dedServ) {
            TryAlternativeTexturePaths(ref _texture);
        }
    }
}