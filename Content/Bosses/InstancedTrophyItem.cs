using Aequus.Core.ContentGeneration;
using Terraria.Localization;

namespace Aequus.Content.Bosses;

internal class InstancedTrophyItem : InstancedTileItem {
    public InstancedTrophyItem(ModTile trophyTile) : base(trophyTile, value: Item.sellPrice(gold: 1), rarity: ItemRarityID.Blue) {
    }

    public override LocalizedText DisplayName => Language.GetOrRegister(_modTile.GetCategoryKey($"Trophies.{Name}.DisplayName"));
    public override LocalizedText Tooltip => LocalizedText.Empty;
}