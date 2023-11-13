using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

[Autoload(false)]
internal class AutoloadedTileItem : ModItem {
    private readonly AequusModTile _modTile;

    public override string Texture => _modTile.Texture + "Item";

    public override string Name => _modTile.Name;

    public override LocalizedText DisplayName => _modTile.GetLocalization("DisplayName");

    public override LocalizedText Tooltip => LocalizedText.Empty;

    protected override bool CloneNewInstances => true;

    private AutoloadedTileItem() {
    }

    public AutoloadedTileItem(AequusModTile modTile) {
        _modTile = modTile;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(_modTile.Type);
    }

    public override void AddRecipes() {
        _modTile.AddItemRecipes(this);
        if (Mod.TryFind<ModItem>(_modTile.Name + "WallItem", out var wallItem)) {
            CreateRecipe()
                .AddIngredient(wallItem, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}