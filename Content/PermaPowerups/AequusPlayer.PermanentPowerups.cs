using Aequus.Content.Dedicated.BeyondCoin;
using Aequus.Content.PermaPowerups.Shimmer;
using Aequus.Core.IO;
using System;

namespace Aequus;

public partial class AequusPlayer {
    [SaveData("NetherStar")]
    public bool usedConvergentHeart;

    [SaveData("ShimmerCoin")]
    public bool usedShimmerCoin;

    [SaveData("CosmicChest")]
    public bool usedCosmicChest;

    [SaveData("NoHitReward")]
    public bool usedMaxHPRespawnReward;

    private void DoPermanentStatBoosts() {
        if (usedCosmicChest) {
            dropRolls += CosmicChest.LuckIncrease;
        }
        if (usedShimmerCoin) {
            ShimmerCoin.UpdatePermanentEffects(Player);
        }
    }

    private void DoPermanentMaxHPRespawn() {
        if (usedMaxHPRespawnReward) {
            Player.statLife = Math.Max(Player.statLife, Player.statLifeMax2);
        }
    }
}