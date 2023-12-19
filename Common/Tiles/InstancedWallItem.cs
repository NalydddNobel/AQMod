using Aequus.Common.Items;
using Aequus.Core.Autoloading;
using Terraria.Localization;

namespace Aequus.Common.Tiles;

internal class InstancedWallItem : InstancedModItem, IPostSetupContent {
    private readonly ModWall _modWall;
    private readonly bool _dropItem;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modWall"></param>
    /// <param name="dropItem">Whether or not the <paramref name="modWall"/> should drop this item.</param>
    public InstancedWallItem(ModWall modWall, bool dropItem = true) : base(modWall.Name, modWall.Texture + "Item") {
        _modWall = modWall;
        _dropItem = dropItem;
    }

    private string KeyPrefix => Name != _modWall.Name ? $"{Name.Replace(_modWall.Name, "")}." : "";
    public override LocalizedText DisplayName => Language.GetOrRegister(_modWall.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(_modWall.GetLocalizationKey(KeyPrefix + "ItemTooltip"), () => "");

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 400;
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = !_dropItem;
    }

    public void PostSetupContent(Aequus aequus) {
        ItemID.Sets.DrawUnsafeIndicator[Type] = !Main.wallHouse[_modWall.Type];
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableWall(_modWall.Type);
    }

    public override void AddRecipes() {
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