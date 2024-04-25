using Terraria.Localization;

namespace Aequus.Common.Items.Dedications;

public record struct AnonymousDedication(Color TextColor) : IDedicationInfo {
    private LocalizedText _lineCache;

    public ModItem Faeling { get; set; }

    public LocalizedText GetDedicatedLine()
        => _lineCache ??= Language.GetText("Mods.Aequus.Items.CommonTooltips.DedicatedItemAnonymous");
}
