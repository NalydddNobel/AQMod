using System.Collections.Generic;
using Terraria.Localization;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace AequusRemake.Core.ContentGeneration;

internal class InstancedBannerItem : InstancedModItem, IOverrideGroupOrder {
    private readonly ModNPC _npc;
    private readonly ModTile _banner;

    public override LocalizedText DisplayName => _npc.GetLocalization("BannerName", () => _npc.DisplayName.Value + " Banner");
    public override LocalizedText Tooltip => Language.GetText("Mods.AequusRemake.Items.CommonTooltips.Banner").WithFormatArgs(_npc.DisplayName);

    public InstancedBannerItem(ModNPC modNPC, ModTile banner) : base(banner.Name, banner.Texture + "Item") {
        _npc = modNPC;
        _banner = banner;
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 30;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 15;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.createTile = _banner.Type;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.silver * 10;
    }

    public void ModifyItemGroup(ref ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary) {
        int? ordering = new JourneySortByTileId(TileID.Banners).ProvideItemGroupOrdering(myGroup, groupDictionary);
        if (ordering != null) {
            myGroup.OrderInGroup = ordering.Value;
        }
    }
}
