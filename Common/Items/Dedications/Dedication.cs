using Terraria.Localization;

namespace Aequus.Common.Items.Dedications;

public class Dedication {
    public record struct Default(string DedicateeName, Color TextColor) : IDedicationInfo {
        private LocalizedText _lineCache;

        public ModItem Faeling { get; set; }

        public LocalizedText GetDedicatedLine() 
            => _lineCache ??= Language.GetText("Mods.Aequus.Items.CommonTooltips.DedicatedItem").WithFormatArgs(DedicateeName);
    }

    public record struct Anonymous(Color TextColor) : IDedicationInfo {
        private LocalizedText _lineCache;

        public ModItem Faeling { get; set; }

        public LocalizedText GetDedicatedLine()
            => _lineCache ??= Language.GetText("Mods.Aequus.Items.CommonTooltips.DedicatedItemAnonymous");
    }
}
