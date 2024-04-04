namespace Aequus.Common.Players;

public class SpawnratesPlayer : ModPlayer {
    public float rateMultiplier;
    public float maxSpawnsDivisor;

    public override void ResetEffects() {
        rateMultiplier = 1f;
        maxSpawnsDivisor = 1f;
    }
}
