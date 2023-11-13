using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

[Autoload(false)]
internal class AutoloadedWallItem : ModItem {
    private readonly AequusModWall _modWall;
    private readonly bool friendly;

    public override string Texture => _modWall.Texture + "Item";

    public override string Name => _modWall.Name + (friendly ? "" : "Hostile");

    public override LocalizedText DisplayName => _modWall.GetLocalization("DisplayName");

    public override LocalizedText Tooltip => LocalizedText.Empty;

    protected override bool CloneNewInstances => true;

    private AutoloadedWallItem() {
    }

    public AutoloadedWallItem(AequusModWall modTile, bool friendly = true) {
        _modWall = modTile;
        this.friendly = friendly;
    }

    public override void SetStaticDefaults() {
        ItemID.Sets.DrawUnsafeIndicator[Type] = !friendly;
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = !friendly;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableWall(_modWall.Type);
    }

    public override void AddRecipes() {
        _modWall.AddItemRecipes(this);
        string modWallName = _modWall.Name;
        if (modWallName.Contains("Wall") && Mod.TryFind<ModItem>(modWallName.Replace("Wall", "") + "Item", out var blockItem)) {
            CreateRecipe(4)
                .AddIngredient(blockItem)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}