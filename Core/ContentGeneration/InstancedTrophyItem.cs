using tModLoaderExtended.Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Aequus.Core.ContentGeneration;

internal class InstancedTrophyItem : InstancedTileItem {
    public InstancedTrophyItem(ModTile trophyTile) : base(trophyTile, value: Item.sellPrice(gold: 1), rarity: ItemRarityID.Blue, journeyOverride: new JourneySortByTileId(TileID.Painting3X3)) { }

    public override LocalizedText DisplayName => Language.GetOrRegister(_modTile.GetCategoryKey($"Trophies.{Name}.DisplayName"));
    public override LocalizedText Tooltip => LocalizedText.Empty;
}