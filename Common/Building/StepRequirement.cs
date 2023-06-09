using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Building {
    public abstract class StepRequirement : ModType, ILocalizedModType {
        public string LocalizationCategory => "Carpenter.StepRequirement";

        protected virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
        protected virtual LocalizedText Description => this.GetLocalization("Description", () => "");

        private LocalizedText _displayName;
        private LocalizedText _description;

        public LocalizedText GetDisplayName() => _displayName;
        public LocalizedText GetDescription() => _description;

        protected sealed override void Register() {
            _displayName = DisplayName;
            _description = Description;
        }

        public sealed override void SetupContent() {
            SetStaticDefaults();
        }
    }

    public abstract class StepRequirement<T, T2> : StepRequirement where T : struct, IScanInfo where T2 : struct, IStepRequirementParameters {
        public abstract IStepResults GetStepResults(in T info, in T2 parameters = default);
    }

    public abstract class StepRequirement<T> : StepRequirement<T, NoParameters> where T : struct, IScanInfo  {
    }
}