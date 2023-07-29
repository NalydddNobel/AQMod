using Aequus.Common.Carpentry;
using Aequus.Common.Carpentry.Results;
using Aequus.Content.Building.Bonuses;
using Aequus.Content.Building.Passes;
using Aequus.Items.Tools.Building;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Building.Challenges;

public class FountainChallenge : BuildChallenge {
    #region WaterfallScan
    private static void Waterfall(int i, int j, int xDir, ScanMap<bool> map, in ScanInfo info) {
        map.SafeSet(i, j, true);
        for (int k = 0; k < 80; k++) {
            map.SafeSet(i, j, true);
            if (info[i, j].LiquidAmount > 0) {
                return;
            }

            if (info[i, j + 1].IsFullySolid()) {
                if (xDir == 0) {
                    return;
                }
                if (info[i + xDir, j].IsFullySolid() && !info[i, j].IsHalfBlock) {
                    // Loop
                    if (info[i - xDir, j].IsFullySolid() && !info[i, j].IsHalfBlock) {
                        return;
                    }

                    xDir = -xDir;
                }
                i += xDir;
                continue;
            }
            j++;
        }
    }

    private static bool IsSolidHalfBrick(int i, int j, in ScanInfo info) {
        return info[i, j].IsFullySolid() && info[i, j].IsHalfBlock;
    }

    private static void CheckWaterTile(int i, int j, ScanMap<bool> map, in ScanInfo info) {
        if (IsSolidHalfBrick(i - 1, j, in info)) {
            Waterfall(i - 1, j, -1, map, in info);
        }
        if (IsSolidHalfBrick(i + 1, j, in info)) {
            Waterfall(i + 1, j, 1, map, in info);
        }
        if (IsSolidHalfBrick(i, j + 1, in info)) {
            Waterfall(i, j + 1, 0, map, in info);
        }
    }

    public static void ScanWaterfalls(ScanMap<bool> map, in ScanInfo info) {
        for (int i = 0; i < info.Width; i++) {
            for (int j = 0; j < info.Height; j++) {
                if (info[i, j].LiquidAmount <= 0) {
                    continue;
                }

                CheckWaterTile(i, j, map, in info);
            }
        }
    }
    #endregion

    public override IEnumerable<Item> GetRewards() {
        return new Item[] {
            new(ModContent.ItemType<AdvancedRuler>())
        };
    }

    public override int BuildBuffType => ModContent.BuffType<FountainBountyBuff>();
    public override int BountyNPCType => NPCID.Guide;

    public override StepRequirement[] LoadSteps() {
        return new StepRequirement[] {
            ModContent.GetInstance<WaterfallHeightStep>(),
            ModContent.GetInstance<CraftableTileStep>()
        };
    }

    protected override void DoScan(IStepResults[] results, ref HighlightInfo highlight, in ScanInfo info) {
        ScanWaterfalls(highlight.InterestMap, in info);
        results[0] = ModContent.GetInstance<WaterfallHeightStep>().GetStepResults(in info, new(7, highlight.InterestMap));
        var shapeScanBox = highlight.InterestMap.GetBoundsRectangle(i => i);
        if (!shapeScanBox.IsEmpty) {
            shapeScanBox.Inflate(3, 3);
        }
        results[1] = ModContent.GetInstance<CraftableTileStep>().GetStepResults(in info, new(20, shapeScanBox, highlight.ShapeMap, highlight.ErrorMap));
    }
}