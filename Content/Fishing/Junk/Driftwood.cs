using Aequus.Common.DataSets;

namespace Aequus.Content.Fishing.Junk;

public class Driftwood : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.ExtractinatorMode[Type] = ItemID.OldShoe;
        ItemSets.FishingTrashForDevilsTounge.Add(Type);
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishingSeaweed);
        Item.width = 10;
        Item.height = 10;
    }
}