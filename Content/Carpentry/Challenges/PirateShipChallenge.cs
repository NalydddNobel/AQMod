using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using Aequus.Content.Carpentry.Bonuses;
using Aequus.Content.Carpentry.Passes;
using Aequus.Items.Tools;
using Aequus.NPCs.Town.ExporterNPC;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Carpentry.Challenges;

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

    public override void PopulateScanResults(IScanResults[] results, ref HighlightInfo highlightInfo, in ScanInfo scanInfo) {
        var houseSearchParameters = new FindHousesStep.Parameters(2, 40, scanInfo.Area, highlightInfo.ShapeMap, highlightInfo.ErrorMap, FindHousesStep.SearchResultType.CountHouses);
        results[0] = ModContent.GetInstance<AtOceanStep>().GetStepResults(in scanInfo);
        results[1] = ModContent.GetInstance<FindHousesStep>().GetStepResults(in scanInfo, houseSearchParameters);
        var countFurnitureParameters = new CountFurnitureStep.Parameters(12, highlightInfo.InterestMap, highlightInfo.ErrorMap);
        countFurnitureParameters.AddPoints(houseSearchParameters.FoundHouses);
        results[2] = ModContent.GetInstance<CountFurnitureStep>().GetStepResults(in scanInfo, countFurnitureParameters);
    }

    public override void PopulateStepDescriptions(string[] descriptions) {
        descriptions[0] = ModContent.GetInstance<AtOceanStep>().GetDescription().Value;
        descriptions[1] = ModContent.GetInstance<FindHousesStep>().GetDescription().Format(2);
        descriptions[2] = ModContent.GetInstance<CountFurnitureStep>().GetDescription().Format(12);
    }
}