namespace Aequus.Content.Fishing.Junk;

public class PlasticBottle : ModItem {
    public override void SetStaticDefaults() {
        ItemID.Sets.ExtractinatorMode[Type] = ItemID.OldShoe;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FishingSeaweed);
        Item.width = 10;
        Item.height = 10;
    }
}