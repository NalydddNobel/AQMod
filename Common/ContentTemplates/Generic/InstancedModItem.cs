using Terraria.Localization;

namespace Aequus.Common.ContentTemplates.Generic;

[Autoload(false)]
internal abstract class InstancedModItem : ModItem {
    protected readonly string _name;
    protected readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    private static void TryAlternativeTexturePaths(ref string texture) {
        if (ModContent.HasAsset(texture)) {
            return;
        }

        int i = texture.LastIndexOf('/');
        if (i == -1) {
            return;
        }

        string name = texture[(i + 1)..];
        string path = texture[..i];

        string tryFolder = path + "/Items/" + name;
        if (ModContent.HasAsset(tryFolder)) {
            texture = tryFolder;
            return;
        }
        if (tryFolder.EndsWith("Item")) {
            tryFolder = tryFolder[..^4];
            if (ModContent.HasAsset(tryFolder)) {
                texture = tryFolder;
                return;
            }
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

/// <param name="modWall"></param>
/// <param name="dropItem">Whether or not the <paramref name="modWall"/> should drop this item.</param>
internal class InstancedWallItem(ModWall modWall, bool dropItem = true) : InstancedModItem(modWall.Name, modWall.Texture + "Item") {
    private string KeyPrefix => Name != modWall.Name ? $"{Name.Replace(modWall.Name, "")}." : "";
    public override LocalizedText DisplayName => Language.GetOrRegister(modWall.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(modWall.GetLocalizationKey(KeyPrefix + "ItemTooltip"), () => "");

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 400;
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = !dropItem;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableWall(modWall.Type);
    }

    public override void AddRecipes() {
        string modWallName = modWall.Name;
        if (modWallName.Contains("Wall") && Mod.TryFind<ModItem>(modWallName.Replace("Wall", ""), out var blockItem)) {
            CreateRecipe(4)
                .AddIngredient(blockItem)
                .AddTile(TileID.WorkBenches)
                .Register()
                .DisableDecraft();
        }
    }
}
