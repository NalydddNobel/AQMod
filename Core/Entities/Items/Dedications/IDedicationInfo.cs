using Terraria.Localization;

namespace Aequu2.Core.Entities.Items.Dedications;

public interface IDedicationInfo {
    Color TextColor { get; }
    Color FaelingColor => TextColor;
    LocalizedText GetDedicatedLine();

    ModItem Faeling { get; set; }
}
