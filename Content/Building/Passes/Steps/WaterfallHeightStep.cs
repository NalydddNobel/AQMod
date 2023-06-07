using Aequus.NPCs.Town.CarpenterNPC.Quest.Bounties;

namespace Aequus.Content.Building.Passes.Steps {
    public class WaterfallHeightStep : Step {
        public int MinimumHeight;

        public WaterfallHeightStep(int minHeight) : base() {
            MinimumHeight = minHeight;
        }

        protected override void Init(StepInfo info) {
            info.AddInterest(new WaterfallSearchStep.WaterfallInterest());
        }

        protected override StepResult ProvideResult(StepInfo info) {
            var waterfalls = info.GetInterest<WaterfallSearchStep.WaterfallInterest>();
            waterfalls.Update(info);
            return new StepResult("ShortWaterfalls") {
                success = waterfalls.resultRectangle.Height >= MinimumHeight - 1,
                interest = waterfalls.waterfalls,
            };
        }

        public override string GetStepText(CarpenterBounty bounty) {
            return GetStepText(bounty, MinimumHeight);
        }
    }
}