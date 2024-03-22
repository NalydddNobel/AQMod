using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Tiles.Statues;

public class AncientAngelStatue : StatueTileTemplate {
    public ModItem ItemDrop { get; private set; }

    public override void Load() {
        ItemDrop = new InstancedTileItem(this, value: Item.sellPrice(copper: 60));
        Mod.AddContent(ItemDrop);
    }

    protected override void SafeSetStaticDefaults() {
        ItemSets.ShimmerCountsAsItem[ItemDrop.Type] = ItemID.AngelStatue;
    }
}
