namespace Aequus.Content.Fishing;

public abstract class ModBait : ModItem {
    public override string LocalizationCategory => "Fishing.Bait";

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }
}
