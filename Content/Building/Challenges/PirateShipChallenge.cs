using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using Aequus.Content.Building.Bonuses;
using Aequus.Content.Building.Passes;
using Aequus.Items.Tools;
using Aequus.NPCs.Town.ExporterNPC;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Building.Challenges;

public class PirateShipChallenge : BuildChallenge {
    public override int BuildBuffType => ModContent.BuffType<PirateBountyBuff>();
    public override int BountyNPCType => ModContent.NPCType<Exporter>();

    public override IEnumerable<Item> GetRewards() {
        return new Item[] {
            new(ModContent.ItemType<WhiteFlag>())
        };
    }

    public override StepRequirement[] LoadSteps() {
        return new StepRequirement[] {
            ModContent.GetInstance<AtOceanStep>(),
            ModContent.GetInstance<FindHousesStep>(),
            ModContent.GetInstance<CountFurnitureStep>()
        };
    }

    protected override void DoScan(IStepResults[] results, ref HighlightInfo highlightInfo, in ScanInfo scanInfo) {
        var houseSearchParameters = new FindHousesStep.Parameters(2, 40, scanInfo.Area, highlightInfo.ShapeMap, highlightInfo.ErrorMap, FindHousesStep.SearchResultType.CountHouses);
        results[0] = ModContent.GetInstance<AtOceanStep>().GetStepResults(in scanInfo);
        results[1] = ModContent.GetInstance<FindHousesStep>().GetStepResults(in scanInfo, houseSearchParameters);
        var countFurnitureParameters = new CountFurnitureStep.Parameters(12, highlightInfo.InterestMap, highlightInfo.ErrorMap);
        countFurnitureParameters.AddPoints(houseSearchParameters.FoundHouses);
        results[2] = ModContent.GetInstance<CountFurnitureStep>().GetStepResults(in scanInfo, countFurnitureParameters);
    }
}