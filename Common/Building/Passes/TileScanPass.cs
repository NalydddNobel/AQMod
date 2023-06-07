using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Common.Building.Passes {
    public abstract class TileScanPass : ModType, ILocalizedModType {
        public virtual string LocalizationCategory => "Carpenter.BountyStep";

        public int index;

        protected virtual bool IsVisible => true;

        /// <summary>
        /// The translations for the display name of this step.
        /// </summary>
        public virtual LocalizedText DisplayName => this.GetLocalization("DisplayName", PrettyPrintName);
        /// <summary>
        /// The translations for the description of this step.
        /// </summary>
        public virtual LocalizedText Description => this.GetLocalization("Tooltip", () => "");

        public abstract ScanResults Scan(ref ScanInfo info);

        protected sealed override void Register() {
        }
        public sealed override void SetupContent() {
            SetStaticDefaults();
        }
    }
}