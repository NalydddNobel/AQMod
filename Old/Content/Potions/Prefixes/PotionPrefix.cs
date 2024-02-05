using Aequus.Common.ItemPrefixes.Components;
using Aequus.Content.DataSets;

namespace Aequus.Old.Content.Potions.Prefixes;

public abstract class PotionPrefix : ModPrefix, IRemovedByShimmerPrefix {
    public ModItem Item { get; private set; }

    public override void Load() {
        Item = new PotionPrefixItem(this);

        Mod.AddContent(Item);
    }

    public override bool CanRoll(Item item) {
        return ItemSets.Potions.Contains(item.type);
    }
}