using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using Aequus.Content.Building.Bonuses;
using Aequus.Content.Building.Passes;
using Aequus.Items.Tools.Cameras.MapCamera;
using Aequus.Items.Tools.Cameras.MapCamera.Clip;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Building.Challenges;

public class ActuatorDoorChallenge : BuildChallenge {
    public override IEnumerable<Item> GetRewards() {
        return new Item[] {
            new Item(ModContent.ItemType<PixelCamera>()),
            new Item(ModContent.ItemType<PixelCameraClipAmmo>(), 5)
        };
    }

    public override int BuildBuffType => ModContent.BuffType<SecretEntranceBuff>();
    public override int BountyNPCType => NPCID.Mechanic;

    public override StepRequirement[] LoadSteps() {
        return new StepRequirement[] {
            ModContent.GetInstance<FindHousesStep>(),
            ModContent.GetInstance<ActuatorDoorStep>()
        };
    }

    protected override void DoScan(IStepResults[] results, ref HighlightInfo highlight, in ScanInfo info) {
        var parameters = new FindHousesStep.Parameters(1, 30, info.Area, highlight.ShapeMap, highlight.ErrorMap, FindHousesStep.SearchResultType.CountHouses);
        results[0] = ModContent.GetInstance<FindHousesStep>().GetStepResults(in info, parameters);
        results[1] = ModContent.GetInstance<ActuatorDoorStep>().GetStepResults(in info, new(1, parameters.FoundHouses, highlight.InterestMap, highlight.ErrorMap));
    }
}