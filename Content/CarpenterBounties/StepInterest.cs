﻿namespace Aequus.Content.CarpenterBounties
{
    public abstract class StepInterest
    {
        public bool hasCompiledPoints;

        public void Update(StepInfo info)
        {
            if (!hasCompiledPoints)
            {
                CompileInterestingPoints(info);
                hasCompiledPoints = true;
            }
        }

        public abstract void CompileInterestingPoints(StepInfo info);
    }
}