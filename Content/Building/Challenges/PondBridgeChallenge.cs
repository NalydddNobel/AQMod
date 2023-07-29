using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using Aequus.Content.Building.Bonuses;
using Aequus.Content.Building.Passes;
using Aequus.Tiles.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Building.Challenges;

public class PondBridgeChallenge : BuildChallenge {
    public override int BuildBuffType => ModContent.BuffType<BridgeBountyBuff>();

    public override int BountyNPCType => NPCID.Merchant;

    public override IEnumerable<Item> GetRewards() {
        return new Item[] {
            new(ModContent.ItemType<FishSign>())
        };
    }

    public override StepRequirement[] LoadSteps() {
        return new StepRequirement[] {
            ModContent.GetInstance<FindBridgeStep>(),
            ModContent.GetInstance<CountFurnitureStep>(),
            ModContent.GetInstance<CraftableTileStep>(),
        };
    }

    protected override void DoScan(IStepResults[] results, ref HighlightInfo highlightInfo, in ScanInfo scanInfo) {
        FindBridgeStep.Parameters parameters = new(5, 60, LiquidID.Water, 16, scanInfo.Area, highlightInfo.ShapeMap, highlightInfo.ErrorMap);
        results[0] = ModContent.GetInstance<FindBridgeStep>().GetStepResults(in scanInfo, parameters);
        var bridgeLocation = parameters.BridgeLocation.Value;
        results[1] = ModContent.GetInstance<CountFurnitureStep>().GetStepResults(in scanInfo, new CountFurnitureStep.Parameters(4, highlightInfo.InterestMap, highlightInfo.ErrorMap).AddRectangle(bridgeLocation));
        bridgeLocation.X -= scanInfo.X;
        bridgeLocation.Y -= scanInfo.Y;
        results[2] = ModContent.GetInstance<CraftableTileStep>().GetStepResults(in scanInfo, new(20, bridgeLocation, highlightInfo.ShapeMap, highlightInfo.ErrorMap));
    }
}