namespace Aequus.Content.Fishing;

public class FishCatches : ILoadable {
    public ModItem Killifish { get; private set; }
    public ModItem Piraiba { get; private set; }

    public void Load(Mod mod) {
        Killifish = new InstancedFishItem("Killifish", ItemRarityID.Blue, Item.silver * 15, InstancedFishItem.SeafoodDinnerRecipe);
        Piraiba = new InstancedFishItem("Piraiba", ItemRarityID.Blue, Item.silver * 15, null);

        mod.AddContent(Killifish);
        mod.AddContent(Piraiba);
    }

    public void Unload() {
        Killifish = null;
        Piraiba = null;
    }
}
