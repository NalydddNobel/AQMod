#if POLLUTED_OCEAN
namespace Aequus.Content.Fishing.Fish;

public class Killifish : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.SpecularFish);
    }
}
#endif
