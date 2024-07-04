namespace AequusRemake.Core.ContentGeneration;

public abstract class UnifiedModBait : ModItem {
    public override string LocalizationCategory => "Fishing.Bait";

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 5;
    }
}
