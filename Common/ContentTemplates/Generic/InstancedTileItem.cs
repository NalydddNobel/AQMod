using Aequus.Common.Utilities;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Common.ContentTemplates.Generic;

/// <param name="modTile"></param>
/// <param name="style"></param>
/// <param name="nameSuffix">Extra text added to the end of the name.</param>
/// <param name="Settings">Various settings which changes the resulting tile item.</param>
internal class InstancedTileItem(ModTile modTile, int style = 0, string nameSuffix = "", TileItemSettings? Settings = null)
    : InstancedModItem(modTile.Name + nameSuffix, Settings?.Texture ?? ((modTile is InstancedModTile instancedModTile ? instancedModTile._texture : modTile.Texture) + nameSuffix + "Item")), IPostSetupContent {
    [CloneByReference]
    internal readonly ModTile _modTile = modTile;

    public override string LocalizationCategory => "Tiles";

    private string KeyPrefix => Name != _modTile.Name ? $"{Name.Replace(_modTile.Name, "")}." : "";
    public override LocalizedText DisplayName => (Settings?.DisplayName?.Value) ?? Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => ALanguage.GetOrEmpty(_modTile.GetLocalizationKey(KeyPrefix + "ItemTooltip"));

    public override void SetStaticDefaults() {
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = Settings?.DisableAutomaticDropItem ?? false;
    }

    public void PostSetupContent(Aequus aequus) {
        Item.ResearchUnlockCount = Settings?.Research ?? (Main.tileFrameImportant[_modTile.Type] ? 1 : 100);
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(Settings?.TileID ?? _modTile.Type, style);
        Item.rare = Settings?.Rare ?? 0;
        Item.value = Settings?.Value ?? 0;
    }

    public override void AddRecipes() {
        if (Mod.TryFind<ModItem>(_modTile.Name + "Wall", out var wallItem)) {
            CreateRecipe()
                .AddIngredient(wallItem, 4)
                .AddTile(TileID.WorkBenches)
                .Register()
                .DisableDecraft();
        }
    }

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        //var groupOverride = journeyOverride?.ProvideItemGroup();
        //if (groupOverride != null) {
        //    itemGroup = groupOverride.Value;
        //}
    }

    public void ModifyItemGroup(ref ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary) {
        //int? sortingOverride = journeyOverride?.ProvideItemGroupOrdering(myGroup, groupDictionary);
        //if (sortingOverride != null) {
        //    myGroup.OrderInGroup = sortingOverride.Value;
        //}
    }
}

internal record class TileItemSettings {
    public string? Texture;
    public Ref<LocalizedText>? DisplayName;
    public bool DisableAutomaticDropItem = false;
    public int Rare = ItemRarityID.White;
    public int Value = 0;
    public int? Research;
    public int? TileID;

    public TileItemSettings AClone() {
        return (TileItemSettings)MemberwiseClone();
    }
}