namespace Aequus.Core.CrossMod;

public abstract class CrossModItem : ModItem {
    protected virtual void SafeAddRecipes() { }

    public string CrossModName { get; init; }

    public CrossModItem() {
        CrossModName = ExtendCrossMod.GetModFromNamespace(GetType());
    }

    public override string LocalizationCategory => $"CrossMod.{CrossModName}.Items";

    public sealed override void AddRecipes() {
        if (ModLoader.HasMod(CrossModName)) {
            SafeAddRecipes();
        }
    }
}
