﻿using Aequus.Common.Items;
using Aequus.Core.Initialization;
using System;
using Terraria.Localization;

namespace Aequus.Common.Tiles;

internal class InstancedTileItem : InstancedModItem, IPostSetupContent {
    protected readonly ModTile _modTile;
    protected readonly int _style;
    private readonly bool _dropItem;
    private readonly int _rarity;
    private readonly int _value;
    private readonly int? _sacrificeCount;

    private Action<ModItem> _setStaticDefaults;
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
    /// <param name="researchSacrificeCount">Research count override.</param>
    public InstancedTileItem(ModTile modTile, int style = 0, string nameSuffix = "", bool dropItem = true, int rarity = ItemRarityID.White, int value = 0, int? researchSacrificeCount = null) 
        : base(modTile.Name + nameSuffix, (modTile is InstancedModTile instancedModTile ? instancedModTile._texture : modTile.Texture) + nameSuffix + "Item") {
        _modTile = modTile;
        _dropItem = dropItem;
        _style = style;
        _rarity = rarity;
        _value = value;
        _sacrificeCount = researchSacrificeCount;
    }

    public InstancedTileItem WithStaticDefaults(Action<ModItem> setStaticDefaultsAction) {
        _setStaticDefaults += setStaticDefaultsAction;
        return this;
    }

    public InstancedTileItem WithRecipe(Action<ModItem> addRecipeAction) {
        _addRecipes += addRecipeAction;
        return this;
    }

    public override string LocalizationCategory => "Tiles";

    private string KeyPrefix => Name != _modTile.Name ? $"{Name.Replace(_modTile.Name, "")}." : "";
    public override LocalizedText DisplayName => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemTooltip"), () => "");

    public override void SetStaticDefaults() {
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = !_dropItem;
        _setStaticDefaults?.Invoke(this);
    }

    public void PostSetupContent(Aequus aequus) {
        Item.ResearchUnlockCount = _sacrificeCount ?? (Main.tileFrameImportant[_modTile.Type] ? 1 : 100);
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(_modTile.Type, _style);
        Item.rare = _rarity;
        Item.value = _value;
    }

    public override void AddRecipes() {
        _addRecipes?.Invoke(this);
        if (Mod.TryFind<ModItem>(_modTile.Name + "Wall", out var wallItem)) {
            CreateRecipe()
                .AddIngredient(wallItem, 4)
                .AddTile(TileID.WorkBenches)
                .Register()
                .DisableDecraft();
        }
    }
}