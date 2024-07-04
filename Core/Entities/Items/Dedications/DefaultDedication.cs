using Terraria.Localization;

namespace AequusRemake.Core.Entities.Items.Dedications;

public record struct DefaultDedication(string DedicateeName, Color TextColor) : IDedicationInfo {
    private LocalizedText _lineCache;

    public ModItem Faeling { get; set; }

    public LocalizedText GetDedicatedLine() {
        return _lineCache ??= Language.GetText("Mods.AequusRemake.Items.CommonTooltips.DedicatedItem").WithFormatArgs(DedicateeName);
    }
}
