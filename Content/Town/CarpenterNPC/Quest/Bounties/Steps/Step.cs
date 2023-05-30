using System;
using System.Collections.Generic;
using Terraria.Localization;

namespace Aequus.Content.Town.CarpenterNPC.Quest.Bounties.Steps {
    public abstract class Step
    {
        private List<StepInterest> interests;
        private List<Action<StepInfo, Step>> AfterSuccessSteps;

        public Step()
        {
            interests = new List<StepInterest>();
            AfterSuccessSteps = new List<Action<StepInfo, Step>>();
        }

        public void Initialize(StepInfo info)
        {
            Init(info);
        }

        protected virtual void Init(StepInfo info)
        {
        }

        public Step AfterSuccess(Action<StepInfo, Step> after)
        {
            AfterSuccessSteps.Add(after);
            return this;
        }

        public StepResult GetResult(CarpenterBounty bounty, StepInfo info)
        {
            var result = ProvideResult(info);
            if (AfterSuccessSteps != null)
            {
                foreach (var step in AfterSuccessSteps)
                {
                    step.Invoke(info, this);
                }
            }
            result.resultMessage = GetFailKey(bounty, result.resultMessage);
            return result;
        }

        protected abstract StepResult ProvideResult(StepInfo info);

        public virtual string GetStepText(CarpenterBounty bounty)
        {
            string key = $"Mods.Aequus.CarpenterBounty.Rule.{GetType().Name}";
            return Language.GetTextValue(key) != key ? key : null;
        }
        protected string GetStepText(CarpenterBounty bounty, params object[] args)
        {
            string key = $"Mods.Aequus.CarpenterBounty.Rule.{GetType().Name}";
            var text = Language.GetTextValue(key, args);
            return text != key ? text : null;
        }

        public virtual string GetFailKey(CarpenterBounty bounty, string key)
        {
            return GetDefaultOrFancyText(bounty, key);
        }

        protected string GetDefaultOrFancyText(CarpenterBounty bounty, string keyName)
        {
            if (bounty != null)
            {
                string fancyKey = $"{bounty.LanguageKey}.{keyName}";
                if (Language.GetTextValue(fancyKey) != fancyKey)
                    return fancyKey;
            }
            string key = $"Mods.Aequus.CarpenterBounty.{keyName}";
            return Language.GetTextValue(key) != key ? key : null;
        }
    }
}