using Terraria.Localization;
using tModLoaderExtended.Terraria.GameContent.Creative;

namespace Aequus.Core.ContentGeneration;

internal class InstancedRelicItem : InstancedTileItem {
    public InstancedRelicItem(ModTile relicTile) : base(relicTile, value: Item.sellPrice(gold: 1), rarity: ItemRarityID.Master, journeyOverride: new JourneySortByTileId(TileID.MasterTrophyBase)) { }

    public override LocalizedText DisplayName => Language.GetOrRegister(_modTile.GetCategoryKey($"Trophies.{Name}.DisplayName"));
    public override LocalizedText Tooltip => LocalizedText.Empty;

    public override void SetDefaults() {
        base.SetDefaults();
        Item.width = 30;
        Item.height = 40;
        Item.master = true;
    }
}