using System.Collections.Generic;
using Terraria.Localization;
using tModLoaderExtended.Terraria.GameContent.Creative;
using tModLoaderExtended.Terraria.ModLoader;

namespace AequusRemake.Core.ContentGeneration;

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

/// <param name="modTile"></param>
/// <param name="style"></param>
/// <param name="nameSuffix">Extra text added to the end of the name.</param>
/// <param name="dropItem">Whether or not the <paramref name="modTile"/> should drop this item.</param>
/// <param name="rarity">Item rarity.</param>
/// <param name="value">Item value.</param>
/// <param name="researchSacrificeCount">Research count override.</param>
/// <param name="journeyOverride">Journey Mode item group override, used to organize tiles all together in the menu. Utilize <see cref="JourneySortByTileId"/> to sort with tiles with a matching tile id, since many tiles do not have item groups, and are instead sorted by tile id.</param>
/// <param name="TileIdOverride">Overrides the place tile id.</param>
internal class InstancedTileItem(ModTile modTile, int style = 0, string nameSuffix = "", bool dropItem = true, int rarity = ItemRarityID.White, int value = 0, int? researchSacrificeCount = null, IJourneySortOverrideProvider journeyOverride = null, int? TileIdOverride = null) : InstancedModItem(modTile.Name + nameSuffix, (modTile is InstancedModTile instancedModTile ? instancedModTile._texture : modTile.Texture) + nameSuffix + "Item"), IPostSetupContent, IOverrideGroupOrder {
    [CloneByReference]
    internal readonly ModTile _modTile = modTile;

    public override string LocalizationCategory => _modTile.LocalizationCategory;

    private string KeyPrefix => Name != _modTile.Name ? $"{Name.Replace(_modTile.Name, "")}." : "";
    public override LocalizedText DisplayName => _modTile.GetLocalization(KeyPrefix + "ItemDisplayName");
    public override LocalizedText Tooltip => XLanguage.GetOrEmpty(_modTile.GetLocalizationKey(KeyPrefix + "ItemTooltip"));

    public override void SetStaticDefaults() {
        ItemSets.DisableAutomaticPlaceableDrop[Type] = !dropItem;
    }

    public void PostSetupContent(Mod mod) {
        Item.ResearchUnlockCount = researchSacrificeCount ?? (Main.tileFrameImportant[_modTile.Type] ? 1 : 100);
    }

    public override void SetDefaults() {
        Item.DefaultToPlaceableTile(TileIdOverride ?? _modTile.Type, style);
        Item.rare = rarity;
        Item.value = value;
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
        var groupOverride = journeyOverride?.ProvideItemGroup();
        if (groupOverride != null) {
            itemGroup = groupOverride.Value;
        }
    }

    public void ModifyItemGroup(ref ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary) {
        int? sortingOverride = journeyOverride?.ProvideItemGroupOrdering(myGroup, groupDictionary);
        if (sortingOverride != null) {
            myGroup.OrderInGroup = sortingOverride.Value;
        }
    }
}

/// <param name="modWall"></param>
/// <param name="dropItem">Whether or not the <paramref name="modWall"/> should drop this item.</param>
internal class InstancedWallItem(ModWall modWall, bool dropItem = true) : InstancedModItem(modWall.Name, modWall.Texture + "Item"), IPostSetupContent {
    private string KeyPrefix => Name != modWall.Name ? $"{Name.Replace(modWall.Name, "")}." : "";
    public override LocalizedText DisplayName => Language.GetOrRegister(modWall.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(modWall.GetLocalizationKey(KeyPrefix + "ItemTooltip"), () => "");

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 400;
        ItemSets.DisableAutomaticPlaceableDrop[Type] = !dropItem;
    }

    void IPostSetupContent.PostSetupContent(Mod mod) {
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
