namespace Aequu2.Content.Dedicated.BeyondCoin;

public class ShimmerCoinNPC : GlobalNPC {
    // Combat Techniques Volume Two
    public override void BuffTownNPC(ref float damageMult, ref int defense) {
        float effectiveness = ShimmerCoin.Effectiveness;
        damageMult += 0.2f * effectiveness;
        defense += (int)(6 * effectiveness);
    }

    // Peddlers Satchel
    public static void PeddlersSatchel(ref int numSlots) {
        numSlots += Helper.ToIntUsingRNGForDecimals(ShimmerCoin.Effectiveness, Main.rand);
    }
}
