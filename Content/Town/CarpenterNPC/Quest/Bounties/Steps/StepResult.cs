using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties.Steps
{
    public struct StepResult
    {
        public bool success;
        public string resultMessage;
        public bool failed => !success;

        public List<StepResult> perStepResults;
        public List<Point> interest;
        public bool Success()
        {
            foreach (var s in perStepResults)
            {
                if (!s.success)
                    return false;
            }
            return true;
        }

        public StepResult(string failMessage)
        {
            success = false;
            resultMessage = failMessage;
            interest = new List<Point>();
            perStepResults = new List<StepResult>();
        }
    }
}