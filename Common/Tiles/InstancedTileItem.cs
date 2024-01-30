﻿using Aequus.Common.Items;
using Aequus.Core.Initialization;
using Terraria.Localization;

namespace Aequus.Common.Tiles;

internal class InstancedTileItem : InstancedModItem, IPostSetupContent {
    protected readonly ModTile _modTile;
    private readonly System.Int32 _style;
    private readonly System.Boolean _dropItem;
    private readonly System.Int32 _rarity;
    private readonly System.Int32 _value;
    private readonly System.Int32? _sacrificeCount;

    /// <param name="modTile"></param>
    /// <param name="style"></param>
    /// <param name="nameSuffix">Extra text added to the end of the name.</param>
    /// <param name="dropItem">Whether or not the <paramref name="modTile"/> should drop this item.</param>
    /// <param name="rarity">Item rarity.</param>
    /// <param name="value">Item value.</param>
    /// <param name="researchSacrificeCount">Research count override.</param>
    public InstancedTileItem(ModTile modTile, System.Int32 style = 0, System.String nameSuffix = "", System.Boolean dropItem = true, System.Int32 rarity = ItemRarityID.White, System.Int32 value = 0, System.Int32? researchSacrificeCount = null)
        : base(modTile.Name + nameSuffix, (modTile is InstancedModTile instancedModTile ? instancedModTile._texture : modTile.Texture) + nameSuffix + "Item") {
        _modTile = modTile;
        _dropItem = dropItem;
        _style = style;
        _rarity = rarity;
        _value = value;
        _sacrificeCount = researchSacrificeCount;
    }

    public override System.String LocalizationCategory => "Tiles";

    private System.String KeyPrefix => Name != _modTile.Name ? $"{Name.Replace(_modTile.Name, "")}." : "";
    public override LocalizedText DisplayName => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemTooltip"), () => "");

    public override void SetStaticDefaults() {
        ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = !_dropItem;
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
        if (Mod.TryFind<ModItem>(_modTile.Name + "Wall", out var wallItem)) {
            CreateRecipe()
                .AddIngredient(wallItem, 4)
                .AddTile(TileID.WorkBenches)
                .Register()
                .DisableDecraft();
        }
    }
}