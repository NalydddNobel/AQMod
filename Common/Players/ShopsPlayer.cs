namespace Aequus.Common.Players;

public class ShopsPlayer : ModPlayer {
    public float priceMultiplier;

    public override void ResetEffects() {
        priceMultiplier = 1f;
    }
}
