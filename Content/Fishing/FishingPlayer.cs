using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Core.Entities.Items.Components;
using Terraria.DataStructures;

namespace Aequus.Content.Fishing;

public class FishingPlayer : ModPlayer {
    private FishingAttempt _fishingAttemptCache;

    public override void Load() {
        HookManager.ApplyAndCacheHook(typeof(PlayerLoader), typeof(FishingPlayer), nameof(PlayerLoader.CatchFish));
        On_Projectile.FishingCheck_RollItemDrop += ItemDropHooks;
        On_Player.ItemCheck_CheckFishingBobber_PullBobber += PullBobberHooks;
    }

    #region Hooks
    private delegate void PlayerLoader_CatchFish_orig(Player player, FishingAttempt attempt, ref int itemDrop, ref int enemySpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition);
    private static void PlayerLoader_CatchFish(PlayerLoader_CatchFish_orig orig, Player player, FishingAttempt attempt, ref int itemDrop, ref int enemySpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
        IModifyFishAttempt pole = attempt.playerFishingConditions.Pole?.ModItem as IModifyFishAttempt;
        IModifyFishAttempt bait = attempt.playerFishingConditions.Bait?.ModItem as IModifyFishAttempt;
        orig(player, attempt, ref itemDrop, ref enemySpawn, ref sonar, ref sonarPosition);
        pole?.PostCatchFish(player, attempt, ref itemDrop, ref enemySpawn, ref sonar, ref sonarPosition);
        bait?.PostCatchFish(player, attempt, ref itemDrop, ref enemySpawn, ref sonar, ref sonarPosition);
    }

    private static void ItemDropHooks(On_Projectile.orig_FishingCheck_RollItemDrop orig, Projectile bobber, ref FishingAttempt fisher) {
        IModifyFishAttempt pole = fisher.playerFishingConditions.Pole?.ModItem as IModifyFishAttempt;
        IModifyFishAttempt bait = fisher.playerFishingConditions.Bait?.ModItem as IModifyFishAttempt;

        if (pole?.PreCatchFish(bobber, ref fisher) != false && bait?.PreCatchFish(bobber, ref fisher) != false) {
            orig(bobber, ref fisher);
        }

    }

    private static void PullBobberHooks(On_Player.orig_ItemCheck_CheckFishingBobber_PullBobber orig, Player player, Projectile bobber, int baitTypeUsed) {
        IOnPullBobber pole = player.HeldItem.ModItem as IOnPullBobber;
        IOnPullBobber bait = ItemLoader.GetItem(baitTypeUsed) as IOnPullBobber;

        if (pole?.PrePullBobber(player, bobber, ref baitTypeUsed) != true && bait?.PrePullBobber(player, bobber, ref baitTypeUsed) != true) {
            orig(player, bobber, baitTypeUsed);
        }

        pole?.PostPullBobber(player, bobber, baitTypeUsed);
        bait?.PostPullBobber(player, bobber, baitTypeUsed);
    }
    #endregion

    public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel) {
        (bait?.ModItem as IModifyFishingPower)?.ModifyFishingPower(Player, fishingRod, ref fishingLevel);
        (fishingRod?.ModItem as IModifyFishingPower)?.ModifyFishingPower(Player, fishingRod, ref fishingLevel);
    }

    public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
        _fishingAttemptCache = attempt;

        if (Player.InModBiome<PollutedOceanBiomeSurface>()) {
            PollutedOceanSystem.CatchSurfaceFish(in attempt, ref itemDrop, ref npcSpawn);
        }
        else if (Player.InModBiome<PollutedOceanBiomeUnderground>()) {
            PollutedOceanSystem.CatchUndergroundFish(in attempt, ref itemDrop, ref npcSpawn);
        }
    }

    public override void ModifyCaughtFish(Item fish) {
        (_fishingAttemptCache.playerFishingConditions.Bait?.ModItem as IModifyFishItem)?.ModifyFishItem(Player, fish);
        (Player.HeldItem?.ModItem as IModifyFishItem)?.ModifyFishItem(Player, fish);
    }
}
