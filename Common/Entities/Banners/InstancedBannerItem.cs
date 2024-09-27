using Aequus.Common.ContentTemplates.Generic;
using Terraria.Localization;

namespace Aequus.Common.Entities.Banners;

internal class InstancedBannerItem(ModNPC Parent, ModTile Banner) : InstancedModItem(Banner.Name, Banner.Texture + "Item")/*, IOverrideGroupOrder*/ {
    public override LocalizedText DisplayName => Parent.GetLocalization("BannerName", () => Parent.DisplayName.Value + " Banner");
    public override LocalizedText Tooltip => Language.GetText("Mods.Aequus.Items.CommonTooltips.Banner").WithFormatArgs(Parent.DisplayName);

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
        Item.createTile = Banner.Type;
        Item.rare = ItemRarityID.Blue;
        Item.value = Item.silver * 10;
    }

#if CREATIVE
    public void ModifyItemGroup(ref ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup myGroup, Dictionary<int, ContentSamples.CreativeHelper.ItemGroupAndOrderInGroup> groupDictionary) {
        int? ordering = new JourneySortByTileId(TileID.Banners).ProvideItemGroupOrdering(myGroup, groupDictionary);
        if (ordering != null) {
            myGroup.OrderInGroup = ordering.Value;
        }
    }
#endif
}
