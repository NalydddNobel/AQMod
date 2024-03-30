using Aequus.Common.Items.Components;
using Aequus.Common.JourneyMode;
using Aequus.Core.Initialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Core.ContentGeneration;

[Autoload(false)]
internal abstract class InstancedModItem : ModItem {
    protected readonly string _name;
    protected readonly string _texture;

    public override string Name => _name;

    public override string Texture => _texture;

    protected override bool CloneNewInstances => true;

    private void TryAlternativeTexturePaths(ref string texture) {
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

internal class InstancedTileItem : InstancedModItem, IPostSetupContent, IOverrideGroupOrder {
    [CloneByReference]
    protected readonly ModTile _modTile;
    private readonly int _style;
    private readonly bool _dropItem;
    private readonly int _rarity;
    private readonly int _value;
    private readonly int? _sacrificeCount;
    private readonly IJourneySortOverrideProvider _journeyOverride;

    /// <param name="modTile"></param>
    /// <param name="style"></param>
    /// <param name="nameSuffix">Extra text added to the end of the name.</param>
    /// <param name="dropItem">Whether or not the <paramref name="modTile"/> should drop this item.</param>
    /// <param name="rarity">Item rarity.</param>
    /// <param name="value">Item value.</param>
    /// <param name="researchSacrificeCount">Research count override.</param>
    /// <param name="journeyOverride">Journey Mode item group override, used to organize tiles all together in the menu. Utilize <see cref="JourneySortByTileId"/> to sort with tiles with a matching tile id, since many tiles do not have item groups, and are instead sorted by tile id.</param>
    public InstancedTileItem(ModTile modTile, int style = 0, string nameSuffix = "", bool dropItem = true, int rarity = ItemRarityID.White, int value = 0, int? researchSacrificeCount = null, IJourneySortOverrideProvider journeyOverride = null)
        : base(modTile.Name + nameSuffix, (modTile is InstancedModTile instancedModTile ? instancedModTile._texture : modTile.Texture) + nameSuffix + "Item") {
        _modTile = modTile;
        _dropItem = dropItem;
        _style = style;
        _rarity = rarity;
        _value = value;
        _sacrificeCount = researchSacrificeCount;
        _journeyOverride = journeyOverride;
    }

    public override string LocalizationCategory => "Tiles";

    private string KeyPrefix => Name != _modTile.Name ? $"{Name.Replace(_modTile.Name, "")}." : "";
    public override LocalizedText DisplayName => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemDisplayName"));
    public override LocalizedText Tooltip => Language.GetOrRegister(_modTile.GetLocalizationKey(KeyPrefix + "ItemTooltip"), () => "");

    public override void SetStaticDefaults() {
        ItemSets.DisableAutomaticPlaceableDrop[Type] = !_dropItem;
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

    public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
        var groupOverride = _journeyOverride?.ProvideItemGroup();
        if (groupOverride != null) {
            itemGroup = groupOverride.Value;
        }
    }

    public void ModifyItemGroup(ref ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary) {
        int? sortingOverride = _journeyOverride?.ProvideItemGroupOrdering(myGroup, groupDictionary);
        if (sortingOverride != null) {
            myGroup.OrderInGroup = sortingOverride.Value;
        }
    }
}

internal class InstancedWallItem : InstancedModItem {
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
        ItemSets.DisableAutomaticPlaceableDrop[Type] = !_dropItem;
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

internal class InstancedCaughtNPCItem : InstancedModItem, IPostSetupContent {
    [CloneByReference]
    private readonly ModNPC _parent;

    public InstancedCaughtNPCItem(ModNPC parent) : base(parent.Name, parent.Texture) {
        _parent = parent;
    }

    public override LocalizedText DisplayName => _parent.DisplayName;
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void Load() {
        ModTypeLookup<ModItem>.RegisterLegacyNames(this, $"{Name}Item");
    }

    public override void SetStaticDefaults() {
        AutoNPCDefaults._npcToCritter.Add(_parent.Type, (short)Type);
    }

    public void PostSetupContent(Aequus aequus) {
        if (Main.npcFrameCount[_parent.Type] > 1) {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue - 1, Main.npcFrameCount[_parent.Type]));
        }
        if (_parent.NPC.lavaImmune) {
            ItemSets.IsLavaImmuneRegardlessOfRarity[Type] = true;
        }
    }

    public override void SetDefaults() {
        Item.DefaultToCapturedCritter(_parent.Type);
        Item.value = Item.sellPrice(silver: 10);
    }
}