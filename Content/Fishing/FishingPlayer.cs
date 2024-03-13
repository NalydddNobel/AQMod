using Aequus.Content.Biomes.PollutedOcean;
using Aequus.Content.Fishing.Components;
using Aequus.Content.Fishing.Fish.BlackJellyfish;
using System;
using Terraria.DataStructures;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace Aequus.Content.Fishing;

public class FishingPlayer : ModPlayer {
    public override void Load() {
        On_Player.ItemCheck_CheckFishingBobber_PullBobber += On_Player_ItemCheck_CheckFishingBobber_PullBobber;
    }

    private static void On_Player_ItemCheck_CheckFishingBobber_PullBobber(On_Player.orig_ItemCheck_CheckFishingBobber_PullBobber orig, Player player, Projectile bobber, int baitTypeUsed) {
        IOnPullBobber fishingRodPullBobberHook = player.HeldItem.ModItem as IOnPullBobber;
        IOnPullBobber baitPullBobberHook = ItemLoader.GetItem(baitTypeUsed) as IOnPullBobber;
        
        if (fishingRodPullBobberHook?.PrePullBobber(player, bobber, ref baitTypeUsed) != true && baitPullBobberHook?.PrePullBobber(player, bobber, ref baitTypeUsed) != true) {
            orig(player, bobber, baitTypeUsed);
        }

        fishingRodPullBobberHook?.PostPullBobber(player, bobber, baitTypeUsed);
        baitPullBobberHook?.PostPullBobber(player, bobber, baitTypeUsed);
    }

    public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition) {
        if (Player.InModBiome<PollutedOceanBiomeSurface>()) {
            PollutedOceanSystem.CatchSurfaceFish(in attempt, ref itemDrop, ref npcSpawn);
        }
        else if (Player.InModBiome<PollutedOceanBiomeUnderground>()) {
            PollutedOceanSystem.CatchUndergroundFish(in attempt, ref itemDrop, ref npcSpawn);
        }
    }
}
