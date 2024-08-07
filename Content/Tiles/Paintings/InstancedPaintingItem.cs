using Aequus.Common.ContentGeneration;
using System;
using Terraria.Localization;

namespace Aequus.Content.Tiles.Paintings;

internal class InstancedPaintingItem(Lazy<LocalizedText>? OverrideName, InstancedPainting Parent) : InstancedTileItem(Parent, 0, rarity: Parent.Rare, value: Parent.Value) {
    public override string Texture => Parent.ItemTexture;

    public override LocalizedText DisplayName => OverrideName?.Value ?? base.DisplayName;
}
