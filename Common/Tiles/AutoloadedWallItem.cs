using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

[Autoload(false)]
internal class AutoloadedWallItem : ModItem {
    private readonly AequusModWall _modWall;
    public readonly bool Friendly;

    public override string Texture => _modWall.Texture + "Item";

    public override string Name => _modWall.Name + (Friendly ? "" : "Hostile");

    public override LocalizedText DisplayName => _modWall.GetLocalization((Friendly ? "DisplayName" : "HostileName"));

    public override LocalizedText Tooltip => LocalizedText.Empty;

    protected override bool CloneNewInstances => true;

    private AutoloadedWallItem() {
    }

    public AutoloadedWallItem(AequusModWall modTile, bool friendly = true) {
        _modWall = modTile;
        this.Friendly = friendly;
    }

    public override void SetStaticDefaults() {
        if (!Friendly && Mod.TryFind<ModItem>(_modWall.Name, out var friendlyWall)) {
            ItemID.Sets.ShimmerTransformToItem[friendlyWall.Type] = Type;
            ItemID.Sets.ShimmerTransformToItem[Type] = friendlyWall.Type;
        }
        ItemID.Sets.DrawUnsafeIndicator[Type] = !Friendly;
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = !Friendly;

        Item.ResearchUnlockCount = 400;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableWall(_modWall.Type);
    }

    public override void AddRecipes() {
        _modWall.AddItemRecipes(this);
        if (!Friendly) {
            return;
        }
        string modWallName = _modWall.Name;
        if (modWallName.Contains("Wall") && Mod.TryFind<ModItem>(modWallName.Replace("Wall", ""), out var blockItem)) {
            CreateRecipe(4)
                .AddIngredient(blockItem)
                .AddTile(TileID.WorkBenches)
                .Register()
                .DisableDecraft();
        }
    }
}