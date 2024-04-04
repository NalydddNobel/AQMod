﻿using Aequus.Common.Players;
using Aequus.Old.Common.Carpentry;
using Aequus.Old.Common.Carpentry.Results;
using Aequus.Old.Content.Carpentry.Bonuses;
using Aequus.Old.Content.Carpentry.Passes;
using System.Collections.Generic;

namespace Aequus.Old.Content.Carpentry.Challenges.PirateShip;

public class PirateShipChallenge : BuildChallenge {
    public override int BuildBuffType => ModContent.BuffType<PirateBountyBuff>();
    public override int BountyNPCType => /*ModContent.NPCType<Exporter>()*/NPCID.Pirate;

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

    public override void UpdateBuff(Player player, ref int buffIndex) {
        player.fishingSkill += 15;
        if (player.TryGetModPlayer(out ShopsPlayer shops)) {
            shops.priceMultiplier *= 0.9f;
        }
    }
}