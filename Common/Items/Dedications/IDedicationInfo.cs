using Terraria.Localization;

namespace Aequus.Common.Items.Dedications;

public interface IDedicationInfo {
    Color TextColor { get; }
    Color FaelingColor => TextColor;
    LocalizedText GetDedicatedLine();

    ModItem Faeling { get; set; }
}
