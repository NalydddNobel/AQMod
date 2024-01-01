using Aequus.Common.Items;
using Terraria.Localization;

namespace Aequus.Content.Tiles.Banners;

internal class InstancedBannerItem : InstancedModItem {
    private readonly ModNPC _npc;
    private readonly ModTile _banner;

    public override LocalizedText DisplayName => _npc.GetLocalization("BannerName", () => _npc.DisplayName.Value + " Banner");
    public override LocalizedText Tooltip => Language.GetText("Mods.Aequus.Items.CommonTooltips.Banner").WithFormatArgs(_npc.DisplayName);

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
}
