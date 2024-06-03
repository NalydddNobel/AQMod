using Terraria.DataStructures;

namespace Aequus.Common.Items.Components;

/// <summary>Only works on Fishing Pole or Bait items.</summary>
internal interface IModifyFishAttempt {
    /// <returns>Return false to prevent vanilla rolling fish</returns>
    bool PreCatchFish(Projectile bobber, ref FishingAttempt fisher) {
        return true;
    }
    void PostCatchFish(Player player, FishingAttempt attempt, ref int itemDrop, ref int enemySpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
    }
}
