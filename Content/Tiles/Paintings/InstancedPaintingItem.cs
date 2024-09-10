using Aequus.Common.ContentTemplates.Generic;
using System;
using Terraria.Localization;

namespace Aequus.Content.Tiles.Paintings;

internal class InstancedPaintingItem(Lazy<LocalizedText>? OverrideName, LocalizedText? OverrideTooltip, InstancedPainting Parent) : InstancedTileItem(Parent, 0, Settings: new TileItemSettings() {
    Value = Parent.Value,
    Rare = Parent.Rare
}) {
    public override string Texture => Parent.ItemTexture;

    public override LocalizedText DisplayName => OverrideName?.Value ?? base.DisplayName;
    public override LocalizedText Tooltip => OverrideTooltip ?? base.Tooltip;
}
