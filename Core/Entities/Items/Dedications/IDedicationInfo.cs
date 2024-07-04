using Terraria.Localization;

namespace AequusRemake.Core.Entities.Items.Dedications;

public interface IDedicationInfo {
    Color TextColor { get; }
    Color FaelingColor => TextColor;
    LocalizedText GetDedicatedLine();

    ModItem Faeling { get; set; }
}
