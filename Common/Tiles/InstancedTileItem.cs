using Aequus.Common.Items;
using Aequus.Core.Autoloading;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Tiles;

internal class InstancedTileItem : InstancedModItem, IPostSetupContent {
    private readonly ModTile _modTile;
    private readonly int _style;
    private readonly bool _dropItem;
    private readonly int _rarity;
    private readonly int _value;

    private Action<ModItem> _addRecipes;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modTile"></param>
    /// <param name="style"></param>
    /// <param name="nameSuffix">Extra text added to the end of the name.</param>
    /// <param name="dropItem">Whether or not the <paramref name="modTile"/> should drop this item.</param>
    /// <param name="rarity">Item rarity.</param>
    /// <param name="value">Item value.</param>
    public InstancedTileItem(ModTile modTile, int style = 0, string nameSuffix = "", bool dropItem = true, int rarity = ItemRarityID.White, int value = 0) : base(modTile.Name + nameSuffix, modTile.Texture + nameSuffix + "Item") {
        _modTile = modTile;
        _dropItem = dropItem;
        _style = style;
        _rarity = rarity;
        _value = value;
    }

    public InstancedTileItem WithRecipe(Action<ModItem> addRecipeAction) {
        _addRecipes += addRecipeAction;
        return this;
    }

    public override string LocalizationCategory => "Tiles";

    private string KeyPrefix => Name != _modTile.Name ? $"{Name.Replace(_modTile.Name, "")}." : "";
    public override LocalizedText DisplayName => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemTooltip"));

    public override void SetStaticDefaults() {
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = _dropItem;
    }

    public void PostSetupContent(Aequus aequus) {
        Item.ResearchUnlockCount = Main.tileFrameImportant[_modTile.Type] ? 1 : 100;
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(_modTile.Type, _style);
        Item.rare = _rarity;
        Item.value = _value;
    }

    public override void AddRecipes() {
        _addRecipes?.Invoke(this);
    }
}