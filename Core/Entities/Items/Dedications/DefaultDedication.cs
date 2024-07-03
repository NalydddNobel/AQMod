using Terraria.Localization;

namespace Aequu2.Core.Entities.Items.Dedications;

public record struct DefaultDedication(string DedicateeName, Color TextColor) : IDedicationInfo {
    private LocalizedText _lineCache;

    public ModItem Faeling { get; set; }

    public LocalizedText GetDedicatedLine() {
        return _lineCache ??= Language.GetText("Mods.Aequu2.Items.CommonTooltips.DedicatedItem").WithFormatArgs(DedicateeName);
    }
}
