using Aequus.Common.DataSets;
using Aequus.Items.Weapons.Ranged.Misc.BlockGlove;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Tooltips {
    public partial class SpecialAbilityTooltips {
        private void AddBlockGloveTooltip(Item item) {
            if (!TileSets.ProjectileInfo.TryGetValue(item.createTile, out var info) || string.IsNullOrEmpty(info.Key)) {
                return;
            }
            SpecialAbilityTooltipInfo tooltip = new(TextHelper.GetItemName<BlockGlove>().Value, Color.SaddleBrown * 2f, ModContent.ItemType<BlockGlove>());
            tooltip.AddLine(Language.GetTextValue(info.Key));
            _tooltips.Add(tooltip);
        }
    }
}