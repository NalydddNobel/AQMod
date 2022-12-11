using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Aequus.Content.CarpenterBounties
{
    public struct StepResult
    {
        public bool success;
        public string resultMessage;
        public bool failed => !success;

        public List<Point> interest;

        public StepResult(string failMessage)
        {
            success = false;
            resultMessage = failMessage;
            interest = new List<Point>();
        }
    }
}