using Aequus.Common.Players;
using Aequus.Old.Common.Carpentry;
using Aequus.Old.Common.Carpentry.Results;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward;
using Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor.Reward.Clip;
using Aequus.Old.Content.Carpentry.Passes;
using System.Collections.Generic;

namespace Aequus.Old.Content.Carpentry.Challenges.ActuatorDoor;

public class ActuatorDoorChallenge : BuildChallenge {
    public override int BountyNPCType => NPCID.Mechanic;

    public override IEnumerable<Item> GetRewards() {
        yield return new Item(ModContent.ItemType<PixelCamera>());
        yield return new Item(ModContent.ItemType<PixelCameraClipAmmo>(), 5);
    }

    public override StepRequirement[] LoadSteps() {
        return new StepRequirement[] {
            ModContent.GetInstance<FindHousesStep>(),
            ModContent.GetInstance<ActuatorDoorStep>()
        };
    }

    public override void PopulateScanResults(IScanResults[] results, ref HighlightInfo highlight, in ScanInfo info) {
        var parameters = new FindHousesStep.Parameters(1, 30, info.Area, highlight.ShapeMap, highlight.ErrorMap, FindHousesStep.SearchResultType.CountHouses);
        results[0] = ModContent.GetInstance<FindHousesStep>().GetStepResults(in info, parameters);
        results[1] = ModContent.GetInstance<ActuatorDoorStep>().GetStepResults(in info, new(1, parameters.FoundHouses, highlight.InterestMap, highlight.ErrorMap));
    }

    public override void PopulateStepDescriptions(string[] descriptions) {
        descriptions[0] = ModContent.GetInstance<FindHousesStep>().GetDescription().Format(1);
        descriptions[1] = ModContent.GetInstance<ActuatorDoorStep>().GetDescription().Format(1);
    }

    public override void UpdateBuff(Player player, ref int buffIndex) {
        if (player.TryGetModPlayer(out SpawnratesPlayer spawnRates)) {
            spawnRates.rateMultiplier *= 0.25f;
            spawnRates.maxSpawnsDivisor += 1f;
        }
        if (player.TryGetModPlayer(out ShopsPlayer shops)) {
            shops.priceMultiplier *= 0.9f;
        }
    }
}