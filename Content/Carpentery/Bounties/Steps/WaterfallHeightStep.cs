namespace Aequus.Content.Carpentery.Bounties.Steps
{
    public class WaterfallHeightStep : Step
    {
        public int MinimumHeight;

        public WaterfallHeightStep(int minHeight) : base()
        {
            MinimumHeight = minHeight;
        }

        protected override void Init(StepInfo info)
        {
            info.AddInterest(new WaterfallSearchStep.WaterfallInterest());
        }

        protected override StepResult ProvideResult(StepInfo info)
        {
            var waterfalls = info.GetInterest<WaterfallSearchStep.WaterfallInterest>();
            waterfalls.Update(info);
            return new StepResult("ShortWaterfalls")
            {
                success = waterfalls.resultRectangle.Height >= MinimumHeight - 1,
                interest = waterfalls.waterfalls,
            };
        }

        public override string GetStepKey(CarpenterBounty bounty)
        {
            return GetStepKey(bounty, MinimumHeight);
        }
    }
}