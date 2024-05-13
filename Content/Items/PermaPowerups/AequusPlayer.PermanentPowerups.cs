using System;

namespace Aequus;

public partial class AequusPlayer {
    private void DoPermanentStatBoosts() {
#if !DEBUG
        if (usedCosmicChest) {
            dropRolls += Content.PermaPowerups.Shimmer.CosmicChest.LuckIncrease;
        }
#endif
    }

    private void DoPermanentMaxHPRespawn() {
        if (usedMaxHPRespawnReward) {
            Player.statLife = Math.Max(Player.statLife, Player.statLifeMax2);
        }
    }
}