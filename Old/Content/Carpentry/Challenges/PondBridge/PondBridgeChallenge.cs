using Aequus.Common.Players;
using Aequus.Old.Common.Carpentry;
using Aequus.Old.Common.Carpentry.Results;
using Aequus.Old.Content.Carpentry.Bonuses;
using Aequus.Old.Content.Carpentry.Passes;
using System.Collections.Generic;

namespace Aequus.Old.Content.Carpentry.Challenges.PondBridge;

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

    public override void PopulateScanResults(IScanResults[] results, ref HighlightInfo highlightInfo, in ScanInfo scanInfo) {
        FindBridgeStep.Parameters parameters = new(5, 60, LiquidID.Water, 16, scanInfo.Area, highlightInfo.ShapeMap, highlightInfo.ErrorMap);
        results[0] = ModContent.GetInstance<FindBridgeStep>().GetStepResults(in scanInfo, parameters);
        var bridgeLocation = parameters.BridgeLocation.Value;
        results[1] = ModContent.GetInstance<CountFurnitureStep>().GetStepResults(in scanInfo, new CountFurnitureStep.Parameters(4, highlightInfo.InterestMap, highlightInfo.ErrorMap).AddRectangle(bridgeLocation));
        bridgeLocation.X -= scanInfo.X;
        bridgeLocation.Y -= scanInfo.Y;
        results[2] = ModContent.GetInstance<CraftableTileStep>().GetStepResults(in scanInfo, new(20, bridgeLocation, highlightInfo.ShapeMap, highlightInfo.ErrorMap));
    }

    public override void PopulateStepDescriptions(string[] descriptions) {
        descriptions[0] = ModContent.GetInstance<FindBridgeStep>().GetDescription().Format(5, 60, LiquidID.Water, 16);
        descriptions[1] = ModContent.GetInstance<CountFurnitureStep>().GetDescription().Format(4);
        descriptions[2] = ModContent.GetInstance<CraftableTileStep>().GetDescription().Format(20);
    }

    public override void UpdateBuff(Player player, ref int buffIndex) {
        player.fishingSkill += 5;
        if (player.TryGetModPlayer(out ShopsPlayer shops)) {
            shops.priceMultiplier *= 0.9f;
        }
    }
}